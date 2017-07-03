using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using i18n.Domain.Abstract;
using i18n.Domain.Entities;
using i18n.Helpers;

namespace i18n.Domain.Concrete
{
    public class StringNuggetFinder : INuggetFinder
	{
		private i18nSettings _settings;
        private NuggetParser _nuggetParser;
        private Dictionary<string, string> _strings;

		public StringNuggetFinder(i18nSettings settings, Dictionary<string, string> strings)
		{
            _settings = settings;
            _nuggetParser = new NuggetParser(new NuggetTokens(
			    _settings.NuggetBeginToken,
			    _settings.NuggetEndToken,
			    _settings.NuggetDelimiterToken,
			    _settings.NuggetCommentToken),
                NuggetParser.Context.SourceProcessing);
            _strings = strings;
		}

		/// <summary>
		/// Goes through the Directories to scan recursively and starts a scan of each while that matches the whitelist. (both from settings)
		/// </summary>
		/// <returns>All found nuggets.</returns>
		public IDictionary<string, TemplateItem> ParseAll()
		{
			var templateItems = new ConcurrentDictionary<string, TemplateItem>();
            // Collection of template items keyed by their id.

            foreach(KeyValuePair<string, string> pc in _strings) {
                ParseString(_settings.ProjectDirectory, pc.Value, pc.Key, templateItems);
            }

			return templateItems;
		}

		private void ParseString(string projectDirectory, string content, string context, ConcurrentDictionary<string, TemplateItem> templateItems)
        {
            // Lookup any/all nuggets in the file and for each add a new template item.
            _nuggetParser.ParseString(content, delegate(string nuggetString, int pos, Nugget nugget, string i_entity)
            {
                var referenceContext = ReferenceContext.Create(context, i_entity, pos);

				AddNewTemplateItem(
                    referenceContext,
                    nugget, 
                    templateItems);
                // Done.
                return null; // null means we are not modifying the entity.
            });
        }

	    private void AddNewTemplateItem(ReferenceContext referenceContext, Nugget nugget, ConcurrentDictionary<string, TemplateItem> templateItems)
	    {
            string msgid = nugget.MsgId.Replace("\r\n", "\n").Replace("\r", "\\n");
            // NB: In memory msgids are normalized so that LFs are converted to "\n" char sequence.

            string key = TemplateItem.KeyFromMsgidAndComment(msgid, nugget.Comment, _settings.MessageContextEnabledFromComment);
			List<string> tmpList;
            //
            templateItems.AddOrUpdate(
                key,
                // Add routine.
                k => {
			        TemplateItem item = new TemplateItem();
                    item.MsgKey = key;
			        item.MsgId = msgid;

                    item.References = new List<ReferenceContext> {referenceContext};

			        if (nugget.Comment.IsSet()) {
                        tmpList = new List<string>();
                        tmpList.Add(nugget.Comment);
                        item.Comments = tmpList;
                    }

			        return item;
                },
                // Update routine.
                (k, v) =>
                {
                    var newReferences = new List<ReferenceContext>(v.References.ToList());
                    newReferences.Add(referenceContext);
					v.References = newReferences;

			        if (nugget.Comment.IsSet()) {
					    tmpList = v.Comments != null ? v.Comments.ToList() : new List<string>();
					    tmpList.Add(nugget.Comment);
					    v.Comments = tmpList;
                    }

                    return v;
                });
		}
	}
}
