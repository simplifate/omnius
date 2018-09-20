var DataUpdate = {
  
  init: function() {
    //setInterval($.proxy(this.loadData, this), 20000);
  },
  
  loadData: function() {
    $.ajax("/api/run/" + $("#currentAppName").val() + "/" + $("#currentBlockName").val() + "/?button=loadData", {
      type: 'post',
      data: {},
      success: this.setData
    });
  },
  
  setData: function(data) {
  	  rigstatusdonut_graph_chart.series[0].setData([
        ['online', data.hc_rigstatusdonut.online],
        ['warning', data.hc_rigstatusdonut.warning],
        ['offline', data.hc_rigstatusdonut.offline]
      ], true);
  }
};

$($.proxy(DataUpdate.init, DataUpdate));