	
	function hideShow_menu(p){
		var o = this;
		this.root = p.root;
		this.flag = true;
		this.menuHide = $(p.menuHide);
		this.closeBtn = p.closeBtn;
		this.init = function()
		{
			$(this.closeBtn).on('click', function(event) { 
				if(o.flag) {
					o.hideFormBlock();
					o.flag = false;
				}	
				event.preventDefault();
			});
		}
	}
	// hide/show menu
	hideShow_menu.prototype.hideFormBlock = function(){
		var o = this;
		var width = window.innerWidth;
		if($(o.closeBtn).hasClass('close')){
			$(o.root).velocity("fadeOut");
			$(o.menuHide).velocity("slideUp");
			$(o.closeBtn).removeClass('close');
		}else{
			$(o.root).velocity("fadeIn");
			$(o.menuHide).velocity("slideDown");
			$(o.closeBtn).addClass('close');
		}	
		setTimeout( function() {
			o.flag = true
		}, 100);
	}
	
	function datepicker(){
			
		var date = $('input[type=date]')
			.attr('type', 'text')
			.datepicker({
				dateFormat: 'mm/dd/yy',
				minDate: 0, 
		
		});
		
		var date = new Date();

		var today = new Date();
		var dd = today.getDate();
		var mm = today.getMonth() + 1; //January is 0!
		var yyyy = today.getFullYear();

		if (dd < 10) {
		    dd = '0' + dd
		}

		if (mm < 10) {
		    mm = '0' + mm
		}
		today = mm + '/' + dd + '/' + yyyy;
		$(".form__order input.hasDatepicker").val(today);
	}
	
	