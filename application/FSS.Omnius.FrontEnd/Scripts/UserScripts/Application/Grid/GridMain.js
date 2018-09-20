//var Grid = {
  
//  i: null,
//  g: null,
  
//  init: function() {
//    $('#RigList tbody tr').click(Grid.toggleDetail);
//  },
  
//  toggleDetail: function() {
//    clearInterval(Grid.i);
    
//    if($(this).hasClass('warning')) {
//      $('#RigDetail').html('');
//      $(this).removeClass('warning');
//    }
//    else {
//      Grid.loadDetail.apply(this, []);
//    }
//  },
  
//  loadDetail: function() {
//    var blockName = 'Rig' + $('td', this).eq(1).text().replace(/RIG/, '');
//    var url = '/Grid/' + blockName + ' #RigDetailInfo';
    
//    $('#RigDetail').load(url, Grid.initDetail);
    
//    $('#RigList tbody tr').removeClass('warning');
//    $(this).addClass('warning');
//  },
  
//  initDetail: function() {
//    var timeCell = $('.rig-info td:last-child'); 
//    timeCell.addClass('text-right').html(timeCell.html().replace(/(\d\d:\d\d)/, '<time>' + Grid.getTime() + '</time>'));
    
//    Grid.i = setInterval(Grid.update, 5000);
    
//    var data = [];
//    var now = new Date();
//    var start = new Date();
    
//    start.setHours(0);
//    start.setMinutes(0);
//    start.setSeconds(0);
    
//    while(start < now) {
//      data.push({
//        date: start,
//        value: Grid.rnd(950, 1050)
//      });
//      start = new Date(start.getTime() + 5000);
//    }
    
//    Grid.g = d3.select('svg');
//    var margin = { top: 10, right: 0, left: 0, bottom: 0 };
//    var width = $('svg').eq(0).width();
//    var height = $('svg').eq(0).height();
    
//    var x = d3.scaleTime().rangeRound([0, width]);
//    var y = d3.scaleLinear().domain([2000, 0]).rangeRound([0, 280]);
//    var timeFormat = d3.timeFormat('%H:%M:%S');
    
//    var xAxis = d3.axisBottom(x);
//    var yAxis = d3.axisLeft(y);
    
//    var area = d3.area()
//    	.x(function(d) { return x(d.date); })
//    	.y1(function(d) { return y(d.value); });
    
//    var g = Grid.g.append('g').attr('transform', 'translate(40, 10)');
    
//    Grid.g.append('path').datum(data).attr('class', 'area').attr('d', area);
//    Grid.g.append('g').attr('class', 'x axis').attr('transform', 'translate(40, ' + 280 + ')').call(xAxis);
//    Grid.g.append('g').attr('class', 'y axis').call(yAxis);
//  },
      
//  update: function() {
//    $('.rig-info time').html(Grid.getTime());
    
//    $('.rig-detail tbody tr').each(function() {
//      var cells = $(this).find('td');
//      if(cells.eq(0).hasClass('text-success')) {
//        cells.eq(3).html(Grid.rnd(25, 100) + '%');
//        cells.eq(4).html(Grid.rnd(58, 65) + ' °C');
//        cells.eq(5).html(Grid.rnd(30, 100) + '%');
//      }
//    });
//  },
  
//  getTime: function() {
//    var d = new Date();
//    return d.toTimeString().replace(/.*(\d\d:\d\d:\d\d).*/, '$1');
//  },
  
//  rnd: function(minimum, maximum) {
//    return Math.round( Math.random() * (maximum - minimum) + minimum);
//  }
//};

//$(Grid.init);

