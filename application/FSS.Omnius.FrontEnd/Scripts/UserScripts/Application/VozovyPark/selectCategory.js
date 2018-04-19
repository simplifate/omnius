var CarCategorySelect = {
  
  init: function() {
    $(document).on('change', '#kategorie', $.proxy(this._onCategoryChange, this));
  },
  
  
  setData: function(data)
  {
    console.log(data);
  },
  
  loadData: function(selectedCategory, token)
  {
    $.ajax({
      url: '/api/run/VozovyPark/Prehled?button=Cars',
      type: 'POST',
      data: {
        '__RequestVerificationToken': token,
        'kategorie': selectedCategory,
        'button': 'Cars'
      },
      success: $.proxy(this.setData, this)
    });
  },
  
  getAntiForgeryToken: function(selectedCategory)
  {
    var self = this;
    $.ajax({
      url: '/Persona/Account/GetAntiForgeryToken',
      type: 'GET',
      success: function(token) {
        self.loadData(selectedCategory, token);
      }
    });
  },
  
  _onCategoryChange: function(event)
  {
    this.getAntiForgeryToken(event.target.value);
  }
};

$($.proxy(CarCategorySelect.init, CarCategorySelect));

$(function () {
    setTimeout(function () {
        $("#Cars").each(function () {
            var STKWarning = $(this).children("td:nth-child(3)").text();
            if (STKWarning == "True") {
                $(this).css("background-color", "#ffdede")
            }
        })
    }, 1000);
});
