/*
 * SmartTab plugin v2.0
 * jQuery Tab Control Plugin
 * 
 * by Dipu Raj  
 * http://techlaboratory.net/smarttab
 * 
 * License 
 * http://creativecommons.org/licenses/by-sa/3.0/  
 */
 
(function($){
  $.fn.smartTab = function(action) {
    var options = $.extend({}, $.fn.smartTab.defaults, action);
    var args = arguments;    
    
    return this.each(function() {
      var obj = $(this);
      var tabs = $(".wizard-panel", obj),
          tabContainer = $('.stContainer', obj);
      
      if(!obj.data('options')){
        obj.data('options',options);
      }else{
        options = obj.data('options');
      }
      
      // Method calling logic
      if (!action || action === 'init' || typeof action === 'object') {
        init();
      }else if (action === 'showTab') {
        var ar = Array.prototype.slice.call(args,1);
        if(ar[0]==obj.data('curTabIdx')) return false;
        showTab(ar[0]);
        return true;
      } else if (action === 'next') {
          var cur = obj.data('curTabIdx') + 1;
          if (cur >= $(tabs).length) return false;
          showTab(cur);
      } else if (action === 'prev') {
          var cur = obj.data('curTabIdx') - 1;
          if (cur < 0) return false;
          showTab(cur);
      } else if (action === 'autosize') {
          var curTab = tabs.eq(obj.data('curTabIdx'));
          adjustHeight(curTab);
      } else {
          $.error('Method ' + action + ' does not exist');
      }

      function init(){
        $(obj).addClass('stMain');
        setTabs();
        setEvents();            

        var st = options.selected;
        st = (st) ? st : 0;
        showTab(st);                                    
      }
                
      function setTabs(){
        if(tabContainer.length == 0){
          tabContainer = $('<div></div>').addClass('stContainer');
          $(obj).append(tabContainer);
        }
        $(tabs).each(function(){
          $(this).addClass('tabContent').hide();
          tabContainer.append($(this));
        });
      }
                
      function setEvents(){
        $(tabs).bind("click", function(e){
          if(tabs.index(this)==obj.data('curTabIdx')) return false;
          showTab(tabs.index(this));
          return false;
        });
      }
      
      function showTab(idx){
        if(obj.data('isAnimating')) return false;  
        animateTab(idx);

        return true;
      }
      
      function animateTab(idx){
        if(obj.data('isAnimating')) return false;
        var curTab = tabs.eq(obj.data('curTabIdx'));
        var selTab = tabs.eq(idx);
        obj.data('isAnimating',true);
        options.transitionEffect = options.transitionEffect.toLowerCase();
        if($.isFunction(options.onLeaveTab)) {
          if(!options.onLeaveTab.call(this,$(curTab))) return false;
        }

        if(options.transitionEffect == 'hslide'){ // horizontal slide
            var cFLeft,cLLeft,sFLeft,sLLeft,cWidth = tabContainer.width();                    
            if(idx>obj.data('curTabIdx')){ // forward
              cFLeft = 0;
              cLLeft = (cWidth+10) * -1;
              sFLeft = (cWidth+10);
              sLLeft = 0;
            }else{ //backward
              cFLeft = 0;
              cLLeft = (cWidth+10);
              sFLeft = (cWidth * -2) + 10;
              sLLeft = 0;
            }                  
            if(curTab.length>0){
              $(curTab).css("left",cFLeft).animate({left:cLLeft},options.transitionSpeed,options.transitionEasing,function(e){
                  $(this).hide();
                  setTabAnchor(idx, curTab, selTab);
              });                     
            }                     
            $(selTab).css("left",sFLeft).width(cWidth).show().animate({left:sLLeft},options.transitionSpeed,options.transitionEasing,function(e){
                $(this).show();
                setTabAnchor(idx, curTab, selTab);
            });
        }else if(options.transitionEffect == 'vslide'){ // vertical slide
            var cFTop,cLTop,sFTop,sLTop,cHeight = tabContainer.height();
            var curElm = $(curTab);
            var selElm = $(selTab);
            if(idx>obj.data('curTabIdx')){ // forward
              cFTop = 0;
              cLTop = (curElm.height()+10) * -1;
              sFTop = (selElm.height()+10);
              sLTop = 0;
            }else{ //backward
              cFTop = 0;
              cLTop = (curElm.height()+10);
              sFTop = (selElm.height() * -2) + 10;
              sLTop = 0;
            }
            if(curTab.length>0){
              curElm.css("top",cFTop).animate({top:cLTop},options.transitionSpeed,options.transitionEasing,function(e){
                  curElm.hide();
                  setTabAnchor(idx, curTab, selTab);
              });
            }
            selElm.css("top",sFTop).show().animate({top:sLTop},options.transitionSpeed,options.transitionEasing,function(e){
                $(this).show();
                setTabAnchor(idx, curTab, selTab);
            });
        }else if(options.transitionEffect == 'slide'){ // normal slide
            if(curTab.length>0){
              $(curTab).slideUp(options.transitionSpeed,options.transitionEasing,function(){
                  $(selTab).slideDown(options.transitionSpeed, options.transitionEasing, function () {
                      setTabAnchor(idx, curTab, selTab);
                  });
              });
            }else{
                $(selTab).slideDown(options.transitionSpeed,options.transitionEasing,function(){
                    $(this).show();
                    setTabAnchor(idx, curTab, selTab);
                });
            }
        }else if(options.transitionEffect == 'fade'){ // normal fade
            $(curTab).fadeOut(options.transitionSpeed,options.transitionEasing,function(){
              $(selTab).fadeIn(options.transitionSpeed,options.transitionEasing);
              $(this).show();
              setTabAnchor(idx, curTab, selTab);
            });
        }else{ // none
            if(curTab.length>0) $(curTab).hide();
            $(selTab).show();
            setTabAnchor(idx, curTab, selTab);
        }

        return true;
      }
      
      function setTabAnchor(idx, curTab, selTab) {
          curTab.removeClass("sel");
          selTab.addClass("sel");
          obj.data('curTabIdx', idx);
          obj.data('isAnimating', false);
          adjustHeight(selTab);
          if ($.isFunction(options.onShowTab)) {
              if (!options.onShowTab.call(this, $(selTab), idx)) return false;
          }
          return true;
      }

      // Adjust Height of the container
      function adjustHeight(selTab){
          tabContainer.animate({height: $(selTab).height() + 80}, options.transitionSpeed);
      }
    });
  };  

  // Easing Plugin
  $.extend($.easing,
  {
      def: 'easeOutQuad',
      swing: function (x, t, b, c, d) {
          return $.easing[$.easing.def](x, t, b, c, d);
      },
      easeOutQuad: function (x, t, b, c, d) {
          return -c *(t/=d)*(t-2) + b;
      },
      easeOutQuart: function (x, t, b, c, d) {
          return -c * ((t=t/d-1)*t*t*t - 1) + b;
      },
      easeOutQuint: function (x, t, b, c, d) {
          return c*((t=t/d-1)*t*t*t*t + 1) + b;
      },
      easeInOutExpo: function (x, t, b, c, d) {
          if (t==0) return b;
          if (t==d) return b+c;
          if ((t/=d/2) < 1) return c/2 * Math.pow(2, 10 * (t - 1)) + b;
          return c/2 * (-Math.pow(2, -10 * --t) + 2) + b;
      },
      easeInOutElastic: function (x, t, b, c, d) {
          var s=1.70158;var p=0;var a=c;
          if (t==0) return b;if ((t/=d/2)==2) return b+c;if (!p) p=d*(.3*1.5);
          if (a < Math.abs(c)) {a=c;var s=p/4;}
          else var s = p/(2*Math.PI) * Math.asin (c/a);
          if (t < 1) return -.5*(a*Math.pow(2,10*(t-=1)) * Math.sin( (t*d-s)*(2*Math.PI)/p )) + b;
          return a*Math.pow(2,-10*(t-=1)) * Math.sin( (t*d-s)*(2*Math.PI)/p )*.5 + c + b;
      },
      easeInOutBack: function (x, t, b, c, d, s) {
          if (s == undefined) s = 1.70158;
          if ((t/=d/2) < 1) return c/2*(t*t*(((s*=(1.525))+1)*t - s)) + b;
          return c/2*((t-=2)*t*(((s*=(1.525))+1)*t + s) + 2) + b;
      }
  }); 
 
  // Default Properies
  $.fn.smartTab.defaults = {
      selected: 0,  // Selected Tab, 0 = first tab
      transitionEffect:'none', // Effect on navigation, none/hslide/vslide/slide/fade
      transitionSpeed:'400', // Transion animation speed
      transitionEasing:'easeInOutExpo', // Transition animation easing
      autoHeight:true, // Automatically adjust content height
      onLeaveTab: null, // triggers when leaving a tab
      onShowTab: null  // triggers when showing a tab
  };
})(jQuery);
