/**
 * Basic Map
 */
$(function () {
	var embedcode = '';
	var markdown_embedcode='';
    var geoCoding = new GMaps({
        el: '#geocoding',
        lat: 36.353393,
        lng: 127.378902
		//36.353393, 127.378902
    });
	 GMaps.geolocate({
        success:function (position) {
            geoCoding.setCenter(position.coords.latitude, position.coords.longitude);
			markdown_embedcode = '<iframe width="100%" height="100%" frameborder="0" scrolling="no" marginheight="0" marginwidth="0" src="https://maps.google.com/maps?saddr='+position.coords.latitude+','+position.coords.longitude+'&amp;ie=UTF8&amp;ll='+position.coords.latitude+','+position.coords.longitude+'&amp;spn=0.006295,0.008036&amp;z=15&amp;output=embed"></iframe>';
			embedcode = 'https://maps.googleapis.com/maps/api/staticmap?center='+position.coords.latitude+','+position.coords.longitude+'&size=640x500&maptype=hybrid&zoom=15&markers=size:mid%7Ccolor:red%7C'+position.coords.latitude+','+position.coords.longitude+'&sensor=false' + '|https://maps.google.com/maps?saddr='+position.coords.latitude+','+position.coords.longitude+'&amp;ie=UTF8&amp;ll='+position.coords.latitude+','+position.coords.longitude+'&amp;spn=0.006295,0.008036&amp;z=15&amp;output=embed';
			//console.log(embedcode);
        },
        error:function (error) {
            alert('Geolocation failed: ' + error.message);
        },
        not_supported:function () {
            alert("Your browser does not support geolocation");
        }
	});

    $('#geocoding_form').submit(function(e){
        e.preventDefault();
        GMaps.geocode({
            address: $('#address').val().trim(),
            callback: function(results, status){
                if(status=='OK'){
                    var latlng = results[0].geometry.location;
                    geoCoding.setCenter(latlng.lat(), latlng.lng());
                    geoCoding.addMarker({
                        lat: latlng.lat(),
                        lng: latlng.lng()
                    });
					markdown_embedcode = '<iframe width="100%" height="100%" frameborder="0" scrolling="no" marginheight="0" marginwidth="0" src="https://maps.google.com/maps?saddr='+latlng.lat()+','+latlng.lng()+'&amp;ie=UTF8&amp;ll='+latlng.lat()+','+latlng.lng()+'&amp;spn=0.006295,0.008036&amp;z=15&amp;output=embed"></iframe>';
					embedcode = 'https://maps.googleapis.com/maps/api/staticmap?center='+latlng.lat()+','+latlng.lng()+'&size=640x500&maptype=hybrid&zoom=15&markers=size:mid%7Ccolor:red%7C'+latlng.lat()+','+latlng.lng()+'&sensor=false' + '|https://maps.google.com/maps?saddr='+latlng.lat()+','+latlng.lng()+'&amp;ie=UTF8&amp;ll='+latlng.lat()+','+latlng.lng()+'&amp;spn=0.006295,0.008036&amp;z=15&amp;output=embed';
					//console.log(embedcode);
                }
            }
        });
		

    });
	
	$('#insertmap').click(function () {
        //var src = $(this).attr('src');
       // var src = $(this).attr('data-name');
        //console.log($(this).attr('data-name'));
        //$('#media_data', parent.document).attr('value', src);
        //$('#media_data', parent.document).val(src)
        $('#media_data_Map', parent.document).val(embedcode);
		$('#markdown_media_data_Map', parent.document).val(embedcode);
        parent.$('#widgetMedia_Map').modal('hide');
		parent.$('#Media_Map').modal('hide');
        //$('#media_data', parent.document).attr('value', src);
        //$('#media_data', parent.document).val(src)
    });
});

