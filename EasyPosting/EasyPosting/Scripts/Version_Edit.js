var gridster;
var widget = '';
var media_widget = '';
var file_widget = '';
var count = 0;
var count_map = new Array();
var json_stack = new Array();
var stack_idx = 0;
var min_stack_idx = 0;
var max_stack_idx = 0;
var stack_length = 50;
json_stack[0] = '';
var pressed = false;
var start = undefined;
var startX, startWidth;
var selRange;
var title = new Array();
var link_url = new Array();
var description = new Array();
var send_img_src = new Array();
var timer_cnt = 0;
var timer;
var return_id = 0;

$(document).ready(function () {
    var wid = $(window).height();
    $('#all-panel').css("height", parseInt(wid) - 240);
    $('#dy').css("height", parseInt(wid) - 240);
    $('#keyword-panel').css("height", parseInt(wid) - 150);

    context.init({ preventDoubleContext: false });

    context.attach('.inline-menu', [
		{ text: '키워드 검색', href: '#' }
    ]);

    context.settings({ compress: true });
    widget += '<div class="panel panel-default">';
    widget += '	<div class="panel-heading">';
    widget += '  <div class="panel-header-title text-center">';
    widget += '	  <div class="pull-right">';
    widget += '    <button type="button" class="btn btn-default btn-xs" id="widget-Source"><i class="fa fa-code"></i></button>';
    widget += '	   <button class="btn btn-default btn-xs" data-toggle="modal" data-target="#widgetRemove">';
    widget += '	    <span class="glyphicon glyphicon-remove"></span>';
    widget += '	   </button>';
    widget += '	  </div>';
    widget += '	 </div>';
    widget += ' </div>';
    widget += ' <div class="panel-body clear" id="content-editor" mode="visual">';
    widget += '<p>컨텐츠를 입력해주세요.</p>'
    widget += ' </div>';
    widget += '</div>';

    media_widget += '<div class="panel panel-default">';
    media_widget += '	<div class="panel-heading">';
    media_widget += '  <div class="panel-header-title text-center">';
    media_widget += '	  <div class="pull-right">';
    media_widget += '	   <button class="btn btn-default btn-xs" data-toggle="modal" data-target="#widgetRemove">';
    media_widget += '	    <span class="glyphicon glyphicon-remove"></span>';
    media_widget += '	   </button>';
    media_widget += '	  </div>';
    media_widget += '	 </div>';
    media_widget += ' </div>';
    media_widget += ' <div class="panel-body media" id="media_content" mode="visual">';

    file_widget += '<div class="panel panel-default">';
    file_widget += '	<div class="panel-heading">';
    file_widget += '  <div class="panel-header-title text-center">';
    file_widget += '	  <div class="pull-right">';
    file_widget += '    <button type="button" class="btn btn-default btn-xs" id="widget-Source"><i class="fa fa-code"></i></button>';
    file_widget += '	   <button class="btn btn-default btn-xs" data-toggle="modal" data-target="#widgetRemove">';
    file_widget += '	    <span class="glyphicon glyphicon-remove"></span>';
    file_widget += '	   </button>';
    file_widget += '	  </div>';
    file_widget += '	 </div>';
    file_widget += ' </div>';
    file_widget += ' <div class="panel-body clear" id="content-editor" mode="visual">';

    var minWidth = 130;
    var minHeight = 100;
    gridster = $('.dynamic-editor').gridster({
        widget_base_dimensions: [minWidth, minHeight],
        widget_margins: [5, 5],
        max_cols: 6,
        helper: 'clone',
        resize: {
            enabled: true,
            max_size: [6],
            min_size: [1, 1]
        },
        serialize_params: function ($w, wgd) {
            return {
                id: wgd.el[0].id,
                col: wgd.col,
                row: wgd.row,
                size_x: wgd.size_x,
                size_y: wgd.size_y,
                htmlContent: $($w).find('.panel-body').html()
            };
        }
    }).data('gridster');

    var json = $('.output2').text();
    set_content(json);   

    var table_start = "<table width='545' style='border:0px!important;'><tbody>";
    var table_end = "</tbody></table>";
    var td_style = "style='vertical-align:top; border:0px!important; width:91px;'";
    var n_td_style = "style='vertical-align:top; border:0px!important; width:91px;'";
    var s = gridster.serialize();
    var json = JSON.stringify(s);
    var parsed_json = $.parseJSON(json);
    var result = '';
    var max = 0;
    result += table_start;

    $(parsed_json).each(function (set, set_values) {
        if (max < set_values['row'] + set_values['size_y'])
            max = set_values['row'] + set_values['size_y'];
    });

    var map = new Array(1000);
    for (var i = 0; i < 1000; i++) {
        map[i] = new Array(6);
        for (var j = 0; j < 6; j++) {
            map[i][j] = 0;
        }
    }

    for (var i = 1; i < max; i++) {
        for (var j = 1; j <= 6; j++) {
            $(parsed_json).each(function (set, set_values) {
                if (i == set_values['row'] && j == set_values['col'] && map[i][j] != 1 && map[i][j] != 2) {
                    map[i][j] = 2;
                    for (var k = 0; k < set_values['size_y']; k++) {
                        if (map[i + k][j] != 2)
                            map[i + k][j] = 1;
                        for (var n = 0; n < set_values['size_x']; n++) {
                            if (map[i + k][j + n] != 2)
                                map[i + k][j + n] = 1;
                        }
                    }
                }
            });
        }
    }

    for (var i = 1; i < max; i++) {
        result += "<tr height='100' style='border:0px!important;'>";
        for (var j = 1; j <= 6; j++) {
            var size_x, size_y, content, flag = 0;
            $(parsed_json).each(function (set, set_values) {
                if (i == set_values['row'] && j == set_values['col'] && map[i][j] == 2) {
                    flag = 1;
                    size_x = set_values['size_x'];
                    size_y = set_values['size_y'];
                    content = set_values['htmlContent'];
                    var array = content.split('|');
                    if (array[1] == 'map') {
                        var start = '<iframe width="100%" height="100%" frameborder="0" scrolling="no" marginheight="0" marginwidth="0" src="';
                        var end = '"></iframe>';
                        content = start + array[3] + end;
                    }
                    else if (array[1] == 'video') {
                        var start = "<iframe width='100%' height='100%' z-index src='https://www.youtube.com/embed/";
                        var end = "' frameborder='0' allowfullscreen></iframe>";

                        var youtubeRegex = /^.*((youtu.be\/)|(v\/)|(\/u\/\w\/)|(embed\/)|(watch\?))\??v?=?([^#\&\?]*).*/;
                        var youtubeMatch = array[3].match(youtubeRegex);
                        content = start + youtubeMatch[7] + end;
                    }
                    else if (array[1] == 'image') {
                        var content_array = content.split("|");
                        var sizex = content_array[3];
                        var sizey = content_array[5];
                        content = content.replace('width="100%"', "width='" + (91 * parseInt(sizex)) + "px'");
                        content = content.replace('height="100%"', "height='" + (100 * parseInt(sizey)) + "px'");
                    }
                }
            });
            if (map[i][j] == 2 && flag == 1)
                result += "<td id='" + i + "-" + j + "' colspan='" + size_x + "' rowspan='" + size_y + "' " + td_style + ">" + content + "</td>";
            if (map[i][j] == 0) {
                result += "<td id='" + i + "-" + j + "' " + n_td_style + "></td>";
            }
        }
        result += "</tr>";
    }
    result += "<tr height='10' >";
    for (var j = 1; j <= 6; j++) {
        result += "<td " + n_td_style + "><p>&nbsp;</p></td>";
    }
    result += "</tr>";
    result += table_end;

    var title = $('#Title').val();
    if (title == "")
        title = "제목";
    var tag = $('#Slug').val();
    if (tag == "")
        tag = "태그";

    //if (json != "[]" || timer_cnt == 0) {
    $.ajax({
        type: "POST",
        url: "/Editor/Version",
        data: {
            title: title,
            tag: tag,
            description: json,
            save_count: timer_cnt,
            id: return_id,
            table: result
        }
    }).done(function (data) {
        if (data.success === true) {
            if (timer_cnt == 0) {
                return_id = data.id;
                $('.get_return_id').text(return_id);
            }
        }
        else if (data.success == false) {
            $.gritter.add({
                title: "임시 저장 실패!"
            });
        }
    }).fail(function (e) {
        $.gritter.add({
            title: "임시 저장 POST 실패!"
        });
    });

    timer = setInterval(function () {
        timer_cnt++;

        var table_start = "<table width='545' style='border:0px!important;'><tbody>";
        var table_end = "</tbody></table>";
        var td_style = "style='vertical-align:top; border:0px!important; width:91px;'";
        var n_td_style = "style='vertical-align:top; border:0px!important; width:91px;'";
        var s = gridster.serialize();
        var json = JSON.stringify(s);
        var parsed_json = $.parseJSON(json);
        var result = '';
        var max = 0;
        result += table_start;

        $(parsed_json).each(function (set, set_values) {
            if (max < set_values['row'] + set_values['size_y'])
                max = set_values['row'] + set_values['size_y'];
        });

        var map = new Array(1000);
        for (var i = 0; i < 1000; i++) {
            map[i] = new Array(6);
            for (var j = 0; j < 6; j++) {
                map[i][j] = 0;
            }
        }

        for (var i = 1; i < max; i++) {
            for (var j = 1; j <= 6; j++) {
                $(parsed_json).each(function (set, set_values) {
                    if (i == set_values['row'] && j == set_values['col'] && map[i][j] != 1 && map[i][j] != 2) {
                        map[i][j] = 2;
                        for (var k = 0; k < set_values['size_y']; k++) {
                            if (map[i + k][j] != 2)
                                map[i + k][j] = 1;
                            for (var n = 0; n < set_values['size_x']; n++) {
                                if (map[i + k][j + n] != 2)
                                    map[i + k][j + n] = 1;
                            }
                        }
                    }
                });
            }
        }

        for (var i = 1; i < max; i++) {
            result += "<tr height='100' style='border:0px!important;'>";
            for (var j = 1; j <= 6; j++) {
                var size_x, size_y, content, flag = 0;
                $(parsed_json).each(function (set, set_values) {
                    if (i == set_values['row'] && j == set_values['col'] && map[i][j] == 2) {
                        flag = 1;
                        size_x = set_values['size_x'];
                        size_y = set_values['size_y'];
                        content = set_values['htmlContent'];
                        var array = content.split('|');
                        if (array[1] == 'map') {
                            var start = '<iframe width="100%" height="100%" frameborder="0" scrolling="no" marginheight="0" marginwidth="0" src="';
                            var end = '"></iframe>';
                            content = start + array[3] + end;
                        }
                        else if (array[1] == 'video') {
                            var start = "<iframe width='100%' height='100%' z-index src='https://www.youtube.com/embed/";
                            var end = "' frameborder='0' allowfullscreen></iframe>";

                            var youtubeRegex = /^.*((youtu.be\/)|(v\/)|(\/u\/\w\/)|(embed\/)|(watch\?))\??v?=?([^#\&\?]*).*/;
                            var youtubeMatch = array[3].match(youtubeRegex);
                            content = start + youtubeMatch[7] + end;
                        }
                        else if (array[1] == 'image') {
                            var content_array = content.split("|");
                            var sizex = content_array[3];
                            var sizey = content_array[5];
                            content = content.replace('width="100%"', "width='" + (91 * parseInt(sizex)) + "px'");
                            content = content.replace('height="100%"', "height='" + (100 * parseInt(sizey)) + "px'");
                        }
                    }
                });
                if (map[i][j] == 2 && flag == 1)
                    result += "<td id='" + i + "-" + j + "' colspan='" + size_x + "' rowspan='" + size_y + "' " + td_style + ">" + content + "</td>";
                if (map[i][j] == 0) {
                    result += "<td id='" + i + "-" + j + "' " + n_td_style + "></td>";
                }
            }
            result += "</tr>";
        }
        result += "<tr height='10' >";
        for (var j = 1; j <= 6; j++) {
            result += "<td " + n_td_style + "><p>&nbsp;</p></td>";
        }
        result += "</tr>";
        result += table_end;

        var title = $('#Title').val();
        if (title == "")
            title = "제목";
        var tag = $('#Slug').val();
        if (tag == "")
            tag = "태그";

        //if (json != "[]" || timer_cnt == 0) {
        $.ajax({
            type: "POST",
            url: "/Editor/Version",
            data: {
                title: title,
                tag: tag,
                description: json,
                save_count: timer_cnt,
                id: return_id,
                table: result
            }
        }).done(function (data) {
            if (data.success === true) {
                if (timer_cnt == 0) {
                    return_id = data.id;
                    $('.get_return_id').text(return_id);
                }
                else if (timer_cnt > 1) {
                    $.gritter.add({
                        title: "임시 저장이 완료되었습니다!"
                    });
                }
            }
            else if (data.success == false) {
                $.gritter.add({
                    title: "임시 저장 실패!"
                });
            }
        }).fail(function (e) {
            $.gritter.add({
                title: "임시 저장 POST 실패!"
            });
        });
        //}
        //if (timer_cnt == 30)
            //clearInterval(timer);
    }, 60000);
});

$(function () {
    $('#widget-AddWidget-text').click(function () {
        var html = '<div id="widget' + count + '">' + widget + '</div>';
        var w = gridster.add_widget(html, 2, 2);
        var height = $(w).height();
        var ch_height = height - 50;
        $(w).find('.panel-body').css("height", ch_height);

        setWidgetParameters(w);

        //var height = $(w).find('.panel-body').height();
        // + $(w).find('.panel-header-title').height();
        //var units = Math.ceil(height / minHeight);
        //gridster.resize_widget($(w), 2, units);

        var contents = $('.clear').html();
        $('.clear').click(function () {
            gridster.disable().disable_resize();
            $('#' + $('#currentSelectedLI').val()).find("table td").mousedown(function (e) {
                start = $(this);
                pressed = true;
                startX = e.pageX;
                startWidth = $(this).width();
                $(start).addClass("resizing");
                $(start).addClass("noSelect");
            });

            $(document).mousemove(function (e) {
                if (pressed) {
                    $(start).css("width", parseInt(startWidth + (e.pageX - startX)));
                }
            });

            $(document).mouseup(function () {
                if (pressed) {
                    $(start).removeClass("resizing");
                    $(start).removeClass("noSelect");
                    pressed = false;
                    save();
                }
            });
        })
        .blur(function () {
            gridster.enable().enable_resize();
            if (contents != $(this).html()) {
                contents = $(this).html();
                save();
            }
        })
		.mouseleave(function () {
		    gridster.enable().enable_resize();
		})
        .mouseup(function () {
            $('.clear').wysiwyg("edit", true);
            var height = $('#' + $('#currentSelectedLI').val()).height();
            var ch_height = height - 50;
            $('#' + $('#currentSelectedLI').val()).find('.panel-body').css("height", ch_height);
        })
        .focus(function () {
            gridster.enable().enable_resize();
            var height = $('#' + $('#currentSelectedLI').val()).height();
            var ch_height = height - 50;
            $('#' + $('#currentSelectedLI').val()).find('.panel-body').css("height", ch_height);
        });

        count_map[count] = 1;
        count++;
        save();
    });

    $('#widgetMedia_File').on('show.bs.modal', function () {
        $('#media_data_File').val("");
    });
    $('#widgetMedia_Image').on('show.bs.modal', function () {
        $('#media_data_Image').val("");
    });
    $('#widgetMedia_Video').on('show.bs.modal', function () {
        $('#media_data_Video').val("");
    });
    $('#widgetMedia_Map').on('show.bs.modal', function () {
        $('#media_data_Map').val("");
    });
  
    $('#widgetMedia_Rep').on('show.bs.modal', function () {
        $('#media_data_Rep').val("");
    });

    $('#widget-AddWidget-media_Rep').click(function () {
        var src = $('.panel-body').find('img');
        var result = "";
        var flag = 0;
        result += $('#Title').val() + "\n";
        for (var i = 0; i < src.length; i++) {
            if (src[i].id == '|image|') {
                flag = 1;
                var sp = src[i].outerHTML.split('src="');
                var sa = sp[1].split('"');
                result += sa[0] + "\n";
            }
        }
        if (flag != 1) {
            $.gritter.add({
                title: "이미지가 없습니다.!!"
            });            
        }
        else if (flag == 1) {
            $('#media_data_image_src').val(result);
            $('#widgetMedia_Rep').modal('show');
        }
    })

    $('#widget-AddWidget-media_File').click(function () {
        selRange = saveSelection();
    });

    $('#widgetMedia_Rep').on('hidden.bs.modal', function () {
        var src = $('#media_data_Rep').val();
        console.log(src);
        if (src != "") {
            var start = "<img id='|image|' width='100%' height='100%' src='";
            var end = "' />";
            var result = start + src + end;
            var html = '<div id="media_widget' + count + '">' + media_widget + result + '</div></div></div>';
            var w = gridster.add_widget(html, 6, 4);

            var height = $(w).height();
            var ch_height = height - 50;
            $(w).find('.panel-body').css("height", ch_height);
            $(w).find('img').attr('save_width', "|" + $(w).attr('data-sizex') + "|");
            $(w).find('img').attr('save_height', "|" + $(w).attr('data-sizey') + "|");

            setWidgetParameters(w);

            $('.media').mouseenter(function () {
                var height = $(w).height();
                var ch_height = height - 50;
                $(w).find('.panel-body').css("height", ch_height);
                $(w).find('img').attr('save_width', "|" + $(w).attr('data-sizex') + "|");
                $(w).find('img').attr('save_height', "|" + $(w).attr('data-sizey') + "|");
            })
            .mouseleave(function () {
                var height = $(w).height();
                var ch_height = height - 50;
                $(w).find('.panel-body').css("height", ch_height);
                $(w).find('img').attr('save_width', "|" + $(w).attr('data-sizex') + "|");
                $(w).find('img').attr('save_height', "|" + $(w).attr('data-sizey') + "|");
            });
            $('.gs-resize-handle').mouseenter(function () {
                var height = $(w).height();
                var ch_height = height - 50;
                $(w).find('.panel-body').css("height", ch_height);
                $(w).find('img').attr('save_width', "|" + $(w).attr('data-sizex') + "|");
                $(w).find('img').attr('save_height', "|" + $(w).attr('data-sizey') + "|");
            }).mouseleave(function () {
                var height = $(w).height();
                var ch_height = height - 50;
                $(w).find('.panel-body').css("height", ch_height);
                $(w).find('img').attr('save_width', "|" + $(w).attr('data-sizex') + "|");
                $(w).find('img').attr('save_height', "|" + $(w).attr('data-sizey') + "|");
            }).mousemove(function () {
                var height = $(w).height();
                var ch_height = height - 50;
                $(w).find('.panel-body').css("height", ch_height);
                $(w).find('img').attr('save_width', "|" + $(w).attr('data-sizex') + "|");
                $(w).find('img').attr('save_height', "|" + $(w).attr('data-sizey') + "|");
            });

            count_map[count] = 1;
            count++;
            save();
        }
        $('#media_data_image_src').val("");
    });

    $('#custom0').click(function () {
        var selection = window.getSelection();
        var text = selection.toString();
        if (text != "") {
            $('#Keyword_Direct').val(text);
            $('#han-page-container').html("");
            $("#han-page-loader").removeClass("hide");
            $("#han-page-container").removeClass("in");
            $.ajax({
                type: "POST",
                url: "/Editor/Direct_Search",
                traditional: true,
                data: {
                    str_text: text
                }
            }).done(function (data) {
                if (data.success === true) {
                    title = data.title;
                    description = data.description;
                    setSearchResult();
                }
                else if (data.success == false) {
                    $.gritter.add({
                        title: "키워드 추출 오류!"
                    });
                    $("#han-page-loader").addClass("hide");
                    $("#han-page-container").addClass("in");
                }
            }).fail(function (e) {
                $.gritter.add({
                    title: "TEXT POST 실패"
                });                
                $("#han-page-loader").addClass("hide");
                $("#han-page-container").addClass("in");
            });
        }
        else if (text == "") {
            $.gritter.add({
                title: "키워드가 없습니다.!!"
            });            
        }
    });
    $('#widgetMedia_File').on('hidden.bs.modal', function () {
        var src = $('#media_data_File').val();
        if (src != "") {
            var array = src.split('|');
            var start = "<a href='";
            var middle = "' >";
            var end = array[1] + "</a>";
            var result = start + array[0] + middle + end;

            if ($('#currentSelectedLI').val() != "") {
                restoreSelection(selRange);
                $('#' + $('#currentSelectedLI').val()).find('.clear').focus();
                insertTextAtCursor(result);
            }
            else if ($('#currentSelectedLI').val() == "") {
                $.gritter.add({
                    title: "파일 첨부할 텍스트 객체를 선택해주세요."
                });                
            }
            save();
        }
    });

    $('#widgetMedia_Image').on('hidden.bs.modal', function () {
        var src = $('#media_data_Image').val();
        if (src != "") {
            var start = "<img id='|image|' width='100%' height='100%' src='";
            var end = "' />";
            var result = start + src + end;
            var html = '<div id="media_widget' + count + '">' + media_widget + result + '</div></div></div>';
            var w = gridster.add_widget(html, 3, 3);

            var height = $(w).height();
            var ch_height = height - 50;
            $(w).find('.panel-body').css("height", ch_height);
            $(w).find('img').attr('save_width', "|" + $(w).attr('data-sizex') + "|");
            $(w).find('img').attr('save_height', "|" + $(w).attr('data-sizey') + "|");

            setWidgetParameters(w);

            $('.media').mouseenter(function () {
                var height = $(w).height();
                var ch_height = height - 50;
                $(w).find('.panel-body').css("height", ch_height);
                $(w).find('img').attr('save_width', "|" + $(w).attr('data-sizex') + "|");
                $(w).find('img').attr('save_height', "|" + $(w).attr('data-sizey') + "|");
            })
            .mouseleave(function () {
                var height = $(w).height();
                var ch_height = height - 50;
                $(w).find('.panel-body').css("height", ch_height);
                $(w).find('img').attr('save_width', "|" + $(w).attr('data-sizex') + "|");
                $(w).find('img').attr('save_height', "|" + $(w).attr('data-sizey') + "|");
            });
            $('.gs-resize-handle').mouseenter(function () {
                var height = $(w).height();
                var ch_height = height - 50;
                $(w).find('.panel-body').css("height", ch_height);
                $(w).find('img').attr('save_width', "|" + $(w).attr('data-sizex') + "|");
                $(w).find('img').attr('save_height', "|" + $(w).attr('data-sizey') + "|");
            }).mouseleave(function () {
                var height = $(w).height();
                var ch_height = height - 50;
                $(w).find('.panel-body').css("height", ch_height);
                $(w).find('img').attr('save_width', "|" + $(w).attr('data-sizex') + "|");
                $(w).find('img').attr('save_height', "|" + $(w).attr('data-sizey') + "|");
            }).mousemove(function () {
                var height = $(w).height();
                var ch_height = height - 50;
                $(w).find('.panel-body').css("height", ch_height);
                $(w).find('img').attr('save_width', "|" + $(w).attr('data-sizex') + "|");
                $(w).find('img').attr('save_height', "|" + $(w).attr('data-sizey') + "|");
            });

            count_map[count] = 1;
            count++;
            save();
        }
    });

    $('#widgetMedia_Video').on('hidden.bs.modal', function () {
        var src = $('#media_data_Video').val();
        if (src != "") {
            //var start = "<iframe width='100%' height='100%' z-index src='https://www.youtube.com/embed/";
            //var end = "' frameborder='0' allowfullscreen></iframe>";

            //var youtubeRegex = /^.*((youtu.be\/)|(v\/)|(\/u\/\w\/)|(embed\/)|(watch\?))\??v?=?([^#\&\?]*).*/;
            //var youtubeMatch = src.match(youtubeRegex);
            //var result = start + youtubeMatch[7] + end;
            var start = "<img width='100%' height='100%' id='|video|' src='";
            var middle = "' url='|"
            var end = "|' />";
            var array = src.split("|");
            var result = start + array[0] + middle + array[1] + end;
            var html = '<div id="media_widget' + count + '">' + media_widget + result + '</div></div></div>';
            var w = gridster.add_widget(html, 3, 3);

            var height = $(w).height();
            var ch_height = height - 50;
            $(w).find('.panel-body').css("height", ch_height);

            setWidgetParameters(w);

            $('.media').mouseenter(function () {
                var height = $(w).height();
                var ch_height = height - 50;
                $(w).find('.panel-body').css("height", ch_height);
            })
            .mouseleave(function () {
                var height = $(w).height();
                var ch_height = height - 50;
                $(w).find('.panel-body').css("height", ch_height);
            });
            $('.gs-resize-handle').mouseenter(function () {
                var height = $(w).height();
                var ch_height = height - 50;
                $(w).find('.panel-body').css("height", ch_height);
            }).mouseleave(function () {
                var height = $(w).height();
                var ch_height = height - 50;
                $(w).find('.panel-body').css("height", ch_height);
            }).mousemove(function () {
                var height = $(w).height();
                var ch_height = height - 50;
                $(w).find('.panel-body').css("height", ch_height);
            });
            count_map[count] = 1;
            count++;
            save();
        }
    });

    $('#widget-AddWidget-media_Map').click(function () {
        document.getElementById('MapEmb').contentWindow.location.reload(true);
    });

    $('#widgetMedia_Map').on('hidden.bs.modal', function () {
        var src = $('#media_data_Map').val();
        if (src != "") {
            //var html = '<div id="media_widget' + count + '">' + media_widget + src + '</div></div></div>';
            var start = "<img width='100%' height='100%' id='|map|' src='";
            var middle = "' url='|"
            var end = "|' />";
            var array = src.split("|");
            var result = start + array[0] + middle + array[1] + end;
            var html = '<div id="media_widget' + count + '">' + media_widget + result + '</div></div></div>';
            var w = gridster.add_widget(html, 3, 3);

            var height = $(w).height();
            var ch_height = height - 50;
            $(w).find('.panel-body').css("height", ch_height);

            setWidgetParameters(w);

            $('.media').mouseenter(function () {
                var height = $(w).height();
                var ch_height = height - 50;
                $(w).find('.panel-body').css("height", ch_height);
            })
            .mouseleave(function () {
                var height = $(w).height();
                var ch_height = height - 50;
                $(w).find('.panel-body').css("height", ch_height);
            });
            $('.gs-resize-handle').mouseenter(function () {
                var height = $(w).height();
                var ch_height = height - 50;
                $(w).find('.panel-body').css("height", ch_height);
            }).mouseleave(function () {
                var height = $(w).height();
                var ch_height = height - 50;
                $(w).find('.panel-body').css("height", ch_height);
            }).mousemove(function () {
                var height = $(w).height();
                var ch_height = height - 50;
                $(w).find('.panel-body').css("height", ch_height);
            });
            count_map[count] = 1;
            count++;
            save();
        }
    });

    $('#widget-AddWidget-media_Code').click(function () {
        $('#code').val("");
        $('#preview').html("");
    });

    $('#widget-AddWidget-media_Code_result').click(function () {
        var lexer = $('#lexer option:selected').val();
        var style = $('#style option:selected').val();
        var code = $('#code').val();
        var data = "";

        for (var i = 0; i < code.length; i++) {
            data += '1' + code[i];
        }

        $.ajax({
            type: "POST",
            url: "/Editor/Code",
            traditional: true,
            data: {
                lexer: lexer,
                style: style,
                code: data
            }
        }).done(function (data) {
            if (data.success === true) {
                $('#preview').html(data.result);
            }
        }).fail(function (e) {
            $.gritter.add({
                title: "코드 하이라이트 POST 실패!!"
            });            
        });
    });

    $('#mediaWidgetInsert_Code').click(function () {
        var src = $('#preview').html();
        if (src != "") {
            var html = '<div id="code_widget' + count + '">' + media_widget + src + '</div></div></div>';
            var w = gridster.add_widget(html, 5, 5);

            var height = $(w).height();
            var ch_height = height - 50;
            $(w).find('.panel-body').css("height", ch_height);

            setWidgetParameters(w);

            $('.media').mouseenter(function () {
                var height = $(w).height();
                var ch_height = height - 50;
                $(w).find('.panel-body').css("height", ch_height);
            })
            .mouseleave(function () {
                var height = $(w).height();
                var ch_height = height - 50;
                $(w).find('.panel-body').css("height", ch_height);
            });
            $('.gs-resize-handle').mouseenter(function () {
                var height = $(w).height();
                var ch_height = height - 50;
                $(w).find('.panel-body').css("height", ch_height);
            }).mouseleave(function () {
                var height = $(w).height();
                var ch_height = height - 50;
                $(w).find('.panel-body').css("height", ch_height);
            }).mousemove(function () {
                var height = $(w).height();
                var ch_height = height - 50;
                $(w).find('.panel-body').css("height", ch_height);
            });
            count_map[count] = 1;
            count++;
            save();
        }
    });

    $('#content-editor').wysiwyg({ toolbar_selector: "#toolbar", excludes: ['blockquote', 'insertimage', 'code'] });

    $('#mycanvas').mouseup(function () {
        $('#' + $('#currentSelectedLI').val()).find('#widget-Source').click(function () {
            var height = $('#' + $('#currentSelectedLI').val()).height();
            var ch_height = height - 50;
            $('#' + $('#currentSelectedLI').val()).find('.panel-body').css("height", ch_height);

            var source = '';
            var html = '';
            if ($('#' + $('#currentSelectedLI').val()).find('.clear').attr('mode') == 'visual') {
                var width = $('#' + $('#currentSelectedLI').val()).find('.clear').width();
                var height = $('#' + $('#currentSelectedLI').val()).find('.clear').height();
                $('#' + $('#currentSelectedLI').val()).find('.clear').attr('mode', 'html');

                html = $('#' + $('#currentSelectedLI').val()).find('.clear').html();
                $('#' + $('#currentSelectedLI').val()).find('.clear').html($('<textarea/>').css('width', width).css('height', height).val(html));
            }
            else {
                source = $('#' + $('#currentSelectedLI').val()).find('.clear').find('textarea').val();
                $('#' + $('#currentSelectedLI').val()).find('.clear').attr('mode', 'visual');
                $('#' + $('#currentSelectedLI').val()).find('.clear').html(source);
            }
            $('#' + $('#currentSelectedLI').val()).find('#widget-Source').toggleClass('btn-danger');
        });
    });

    var keycode = -1;
    var key_count = 0;
    $('#mycanvas').keydown(function () {
        if (keycode == -1) {
            keycode = event.keyCode;
        }
    });

    $('#mycanvas').keyup(function () {
        if (event.keyCode == keycode || keycode == 229) {
            var outerHeight = $('#' + $('#currentSelectedLI').val()).find('.panel-body').height() - 15;
            var currentsize = $('#' + $('#currentSelectedLI').val()).attr('data-sizey');
            var totalHeight = 0;
            var height = $('#' + $('#currentSelectedLI').val()).height();
            var ch_height = height - 50;
            $('#' + $('#currentSelectedLI').val()).find('.panel-body').css("height", ch_height);
            $('#' + $('#currentSelectedLI').val()).find('.clear').children().each(function () {
                totalHeight = totalHeight + $(this).height() + parseInt($(this).css('margin-bottom'));
            });
            if (totalHeight >= outerHeight + 50) {
                var sub = ((totalHeight - (outerHeight + 50)) / 110) + 1;
                currentsize = parseInt(currentsize) + parseInt(sub);
                gridster.resize_widget($('#' + $('#currentSelectedLI').val()), $('#' + $('#currentSelectedLI').val()).attr('data-sizex'), currentsize);

                var ch_height = parseInt(currentsize) * 110 - 50;
                $('#' + $('#currentSelectedLI').val()).find('.panel-body').css("height", ch_height);
            }
            keycode = -1;
        }
        key_count++;
        if (key_count % 10 == 0) {
            save();
        }
    });

    $('a.fontsize-option').click(function () {
        var outerHeight = $('#' + $('#currentSelectedLI').val()).find('.panel-body').height() - 15;
        var currentsize = $('#' + $('#currentSelectedLI').val()).attr('data-sizey');
        var totalHeight = 0;
        var height = $('#' + $('#currentSelectedLI').val()).height();
        var ch_height = height - 50;
        $('#' + $('#currentSelectedLI').val()).find('.panel-body').css("height", ch_height);

        $('#' + $('#currentSelectedLI').val()).find('.clear').children().each(function () {
            totalHeight = totalHeight + $(this).height() + parseInt($(this).css('margin-bottom'));
        });
        if (totalHeight >= outerHeight + 50) {
            var sub = ((totalHeight - (outerHeight + 50)) / 110) + 1;
            currentsize = parseInt(currentsize) + parseInt(sub);
            gridster.resize_widget($('#' + $('#currentSelectedLI').val()), $('#' + $('#currentSelectedLI').val()).attr('data-sizex'), currentsize);

            var ch_height = parseInt(currentsize) * 110 - 50;
            $('#' + $('#currentSelectedLI').val()).find('.panel-body').css("height", ch_height);
        }
    });

    function obj_sort(id) {
        for (var i = 0; i < count; i++) {
            if (count_map[i] && id != i) {
                if (parseInt($('#widget' + id).attr('data-col')) + parseInt($('#widget' + id).attr('data-sizex')) - 1 >= $('#widget' + i).attr('data-col')) {
                    if ($('#widget' + id).attr('data-col') <= parseInt($('#widget' + i).attr('data-col')) + parseInt($('#widget' + i).attr('data-sizex')) - 1) {
                        if (parseInt($('#widget' + id).attr('data-row')) + parseInt($('#widget' + id).attr('data-row')) - 1 < $('#widget' + i).attr('data-row')) {
                            gridster.enable().enable_resize();
                        }
                    }
                }
            }
        }
    };

    $('#widgetRemoveYes').click(function () {
        var current_id = $('#currentSelectedLI').val();
        var w = $('div[id="' + current_id + '"]');
        gridster.remove_widget($(w));
        $('#currentSelectedLI').val('');
        var strArray = current_id.split('t');

        count_map[strArray[1]] = 0;
        save();
    });

    $('#widget-submit').click(function () {
        //var table_start = "<table width='550' cellspacing='0' cellpadding='0' border='0' style='border:none;border-collapse:collapse;' ><tbody>";
        var table_start = "<table width='545' style='border:0px!important;'><tbody>";
        var table_end = "</tbody></table>";
        var td_style = "style='vertical-align:top; border:0px!important; width:91px;'";
        var n_td_style = "style='vertical-align:top; border:0px!important; width:91px;'";
        var s = gridster.serialize();
        var json = JSON.stringify(s);
        var parsed_json = $.parseJSON(json);
        var result = '';
        var max = 0;
        result += table_start;

        $(parsed_json).each(function (set, set_values) {
            if (max < set_values['row'] + set_values['size_y'])
                max = set_values['row'] + set_values['size_y'];
        });

        var map = new Array(1000);
        for (var i = 0; i < 1000; i++) {
            map[i] = new Array(6);
            for (var j = 0; j < 6; j++) {
                map[i][j] = 0;
            }
        }

        for (var i = 1; i < max; i++) {
            for (var j = 1; j <= 6; j++) {
                $(parsed_json).each(function (set, set_values) {
                    if (i == set_values['row'] && j == set_values['col'] && map[i][j] != 1 && map[i][j] != 2) {
                        map[i][j] = 2;
                        for (var k = 0; k < set_values['size_y']; k++) {
                            if (map[i + k][j] != 2)
                                map[i + k][j] = 1;
                            for (var n = 0; n < set_values['size_x']; n++) {
                                if (map[i + k][j + n] != 2)
                                    map[i + k][j + n] = 1;
                            }
                        }
                    }
                });
            }
        }

        for (var i = 1; i < max; i++) {
            result += "<tr height='100' style='border:0px!important;'>";
            for (var j = 1; j <= 6; j++) {
                var size_x, size_y, content, flag = 0;
                $(parsed_json).each(function (set, set_values) {
                    if (i == set_values['row'] && j == set_values['col'] && map[i][j] == 2) {
                        flag = 1;
                        size_x = set_values['size_x'];
                        size_y = set_values['size_y'];
                        content = set_values['htmlContent'];
                        var array = content.split('|');
                        if (array[1] == 'map') {
                            var start = '<iframe width="100%" height="100%" frameborder="0" scrolling="no" marginheight="0" marginwidth="0" src="';
                            var end = '"></iframe>';
                            content = start + array[3] + end;
                        }
                        else if (array[1] == 'video') {
                            var start = "<iframe width='100%' height='100%' z-index src='https://www.youtube.com/embed/";
                            var end = "' frameborder='0' allowfullscreen></iframe>";

                            var youtubeRegex = /^.*((youtu.be\/)|(v\/)|(\/u\/\w\/)|(embed\/)|(watch\?))\??v?=?([^#\&\?]*).*/;
                            var youtubeMatch = array[3].match(youtubeRegex);
                            content = start + youtubeMatch[7] + end;
                        }
                        else if (array[1] == 'image') {
                            var content_array = content.split("|");
                            var sizex = content_array[3];
                            var sizey = content_array[5];
                            content = content.replace('width="100%"', "width='" + (91 * parseInt(sizex)) + "px'");
                            content = content.replace('height="100%"', "height='" + (100 * parseInt(sizey)) + "px'");
                        }
                    }
                });
                if (map[i][j] == 2 && flag == 1)
                    result += "<td id='" + i + "-" + j + "' colspan='" + size_x + "' rowspan='" + size_y + "' " + td_style + ">" + content + "</td>";
                if (map[i][j] == 0) {
                    result += "<td id='" + i + "-" + j + "' " + n_td_style + "></td>";
                }
            }
            result += "</tr>";
        }
        result += "<tr height='10' >";
        for (var j = 1; j <= 6; j++) {
            result += "<td " + n_td_style + "><p>&nbsp;</p></td>";
        }
        result += "</tr>";
        result += table_end;
        $('.output').text(result);

        $('.output2').text(json);
    });

    $('.toolbar_undo').click(function () {
        call();
    });
    $('.toolbar_redo').click(function () {
        next();
    });

    $('#Keyword_Direct_btn').click(function () {
        var src = $('#Keyword_Direct').val();
        $('#han-page-container').html("");
        $("#han-page-loader").removeClass("hide");
        $("#han-page-container").removeClass("in");
        $.ajax({
            type: "POST",
            url: "/Editor/Direct_Search",
            traditional: true,
            data: {
                str_text: src
            }
        }).done(function (data) {
            if (data.success === true) {
                title = data.title;
                description = data.description;
                setSearchResult();
            }
            else if (data.success == false) {
                $.gritter.add({
                    title: "키워드 추출 오류!"
                });                
                $("#han-page-loader").addClass("hide");
                $("#han-page-container").addClass("in");
            }
        }).fail(function (e) {
            $.gritter.add({
                title: "TEXT POST 실패"
            });            
            $("#han-page-loader").addClass("hide");
            $("#han-page-container").addClass("in");
        });
    });

    $('#Recommend').click(function () {
        $('#han-page-container').html("");
        $("#han-page-loader").removeClass("hide");
        $("#han-page-container").removeClass("in");
        var s = gridster.serialize();
        var json = JSON.stringify(s);
        var parsed_json = $.parseJSON(json);
        var content = '';

        $(parsed_json).each(function (set, set_values) {
            var array = set_values['id'].split('_');
            if (array[0] != 'code')
                content += ' ' + set_values['htmlContent'];
        });
        var objStrip = new RegExp();
        var objStrip2 = new RegExp();
        objStrip = /[<][^>]*[>]/gi;
        objStrip2 = /&(nbsp|amp|quot|lt|gt);/g;
        var result = content.replace(objStrip, "");
        result = result.replace(objStrip2, "");

        //console.log(result);
        //$('#input_text').val(result);
        //$('#text_submit').submit();        
        //var title;
        //var link;
        //var description;
        $.ajax({
            type: "POST",
            url: "/Editor/SaveText",
            traditional: true,
            data: {
                str_text: result
            }
        }).done(function (data) {
            if (data.success === true) {
                title = data.title;
                description = data.description;
                setSearchResult();
            }
            else if (data.success == false) {
                $.gritter.add({
                    title: "키워드 추출 오류!"
                });                
                $("#han-page-loader").addClass("hide");
                $("#han-page-container").addClass("in");
            }
        }).fail(function (e) {
            $.gritter.add({
                title: "TEXT POST 실패"
            });            
            $("#han-page-loader").addClass("hide");
            $("#han-page-container").addClass("in");
        });

    });

    $(window).resize(function () {
        var wid = $(window).height();
        $('#all-panel').css("height", parseInt(wid) - 240);
        $('#dy').css("height", parseInt(wid) - 240);
        $('#keyword-panel').css("height", parseInt(wid) - 150);
    });
});

function setSearchResult() {
    var append_result = "";
    var count = 0;
    var array_re = new Array();
    //append_result += "<div class='panel-group'>";
    array_re[0] = "<div class='panel-group'>";

    for (var i = 0; i < description.length; i++) {
        var url = description[i].split("|");
        //append_result += "<div class='panel panel-inverse'><div class='panel-heading' style='cursor:default!important;'><h4 class='panel-title'><a href='" + url[0] + "' target='_blank'>" + title[i] + "</a></h4></div><div class='panel-body'>" + url[1] + "</div></div>";
        array_re[i] = "<div class='panel panel-inverse'><div class='panel-heading' style='cursor:default!important;'><h4 class='panel-title'><a href='" + url[0] + "' target='_blank'>" + title[i] + "</a></h4></div><div class='panel-body'>" + url[1] + "</div></div>";
        count++;
    }

    //append_result += "</div>";
    array_re[count] = "</div>";
    //$('#han-page-container').append(append_result);   

    for (var i = 0; i < count; i++) {
        $('#han-page-container').append(array_re[i]);
    }

    $("#han-page-loader").addClass("hide");
    $("#han-page-container").addClass("in");
}

function setWidgetParameters(w) {
    $(w).find('.panel').click(function () {
        $('#currentSelectedLI').val($(this).parent('div').attr('id'));
    });
    $(w).find('.gs-resize-handle').click(function () {
        $('#currentSelectedLI').val($(this).parent('div').attr('id'));
    });
    $(w).find('.gs-resize-handle').mouseenter(function () {
        $('#currentSelectedLI').val($(this).parent('div').attr('id'));
    });
}

function save() {
    var s = gridster.serialize();
    stack_idx++;
    if (stack_idx % stack_length == min_stack_idx % stack_length) min_stack_idx++;
    max_stack_idx = stack_idx;
    json_stack[stack_idx % stack_length] = JSON.stringify(s);
    if (min_stack_idx > 2 * stack_length) {
        min_stack_idx -= stack_length;
        stack_idx -= stack_length;
    }
}

function call() {
    if (stack_idx > min_stack_idx) {
        gridster.remove_all_widgets();
        stack_idx--;
        var json = json_stack[stack_idx % stack_length];
        set_content(json);
    }
}

function next() {
    if (stack_idx < max_stack_idx) {
        gridster.remove_all_widgets();
        stack_idx++;
        var json = json_stack[stack_idx % stack_length];
        set_content(json);
    }
}

function set_content(json) {
    var widget2 = '';
    widget2 += '<div class="panel panel-default">';
    widget2 += '	<div class="panel-heading">';
    widget2 += '  <div class="panel-header-title text-center">';
    widget2 += '	  <div class="pull-right">';
    widget2 += '    <button type="button" class="btn btn-default btn-xs" id="widget-Source"><i class="fa fa-code"></i></button>';
    widget2 += '	   <button class="btn btn-default btn-xs" data-toggle="modal" data-target="#widgetRemove">';
    widget2 += '	    <span class="glyphicon glyphicon-remove"></span>';
    widget2 += '	   </button>';
    widget2 += '	  </div>';
    widget2 += '	 </div>';
    widget2 += ' </div>';
    widget2 += ' <div class="panel-body clear" id="content-editor">';


    var media_widget2 = '';
    media_widget2 += '<div class="panel panel-default">';
    media_widget2 += '	<div class="panel-heading">';
    media_widget2 += '  <div class="panel-header-title text-center">';
    media_widget2 += '	  <div class="pull-right">';
    media_widget2 += '	   <button class="btn btn-default btn-xs" data-toggle="modal" data-target="#widgetRemove">';
    media_widget2 += '	    <span class="glyphicon glyphicon-remove"></span>';
    media_widget2 += '	   </button>';
    media_widget2 += '	  </div>';
    media_widget2 += '	 </div>';
    media_widget2 += ' </div>';
    media_widget2 += ' <div class="panel-body media" id="media-content">';

    var parsed_json = $.parseJSON(json);

    for (var i = 0; i < count; i++) {
        count_map[i] = 0;
    }
    count = 0;

    $(parsed_json).each(function (set, set_values) {
        count_map[count] = 1;
        count++;
        var id = set_values['id'];
        var str_array = id.split('w');
        if (str_array[0] == "") {
            var contents = set_values['htmlContent'];
            var html = '<div id="' + set_values['id'] + '">' + widget2 + contents + "</div></div>";
            var w = gridster.add_widget(html, set_values['size_x'], set_values['size_y'], set_values['col'], set_values['row']);
            var height = $(w).height();
            var ch_height = height - 50;
            $(w).find('.panel-body').css("height", ch_height);

            setWidgetParameters(w);
        }
        else if (str_array[0] != "") {
            var contents = set_values['htmlContent'];
            var html = '<div id="' + set_values['id'] + '">' + media_widget2 + contents + "</div></div>";
            var w = gridster.add_widget(html, set_values['size_x'], set_values['size_y'], set_values['col'], set_values['row']);
            var height = $(w).height();
            var ch_height = height - 50;
            $(w).find('.panel-body').css("height", ch_height);

            setWidgetParameters(w);

            if ($(w).find('img').attr("id") == "|image|") {
                $(w).find('img').attr('save_width', "|" + $(w).attr('data-sizex') + "|");
                $(w).find('img').attr('save_height', "|" + $(w).attr('data-sizey') + "|");
            }
        }
    });
    count++;

    var contents = $('.clear').html();
    $('.clear').click(function () {
        gridster.disable().disable_resize();
        $('#' + $('#currentSelectedLI').val()).find("table td").mousedown(function (e) {
            start = $(this);
            pressed = true;
            startX = e.pageX;
            startWidth = $(this).width();
            $(start).addClass("resizing");
            $(start).addClass("noSelect");
        });

        $(document).mousemove(function (e) {
            if (pressed) {
                $(start).width(startWidth + (e.pageX - startX));
            }
        });

        $(document).mouseup(function () {
            if (pressed) {
                $(start).removeClass("resizing");
                $(start).removeClass("noSelect");
                pressed = false;
                save();
            }
        });
    })
    .blur(function () {
        gridster.enable().enable_resize();
        if (contents != $(this).html()) {
            contents = $(this).html();
            save();
        }
    }).mouseleave(function () {
        gridster.enable().enable_resize();
    })
    .mouseup(function () {
        $('.clear').wysiwyg("edit", true);
    })
    .focus(function () {
        gridster.enable().enable_resize();
        var height = $('#' + $('#currentSelectedLI').val()).height();
        var ch_height = height - 50;
        $('#' + $('#currentSelectedLI').val()).find('.panel-body').css("height", ch_height);

    });

    $('.media').mouseenter(function () {
        var height = $('#' + $('#currentSelectedLI').val()).height();
        var ch_height = height - 50;
        $('#' + $('#currentSelectedLI').val()).find('.panel-body').css("height", ch_height);
        if ($('#' + $('#currentSelectedLI').val()).find('img').attr("id") == "|image|") {
            $('#' + $('#currentSelectedLI').val()).find('img').attr('save_width', "|" + $('#' + $('#currentSelectedLI').val()).attr('data-sizex') + "|");
            $('#' + $('#currentSelectedLI').val()).find('img').attr('save_height', "|" + $('#' + $('#currentSelectedLI').val()).attr('data-sizey') + "|");
        }
    })
    .mouseleave(function () {
        var height = $('#' + $('#currentSelectedLI').val()).height();
        var ch_height = height - 50;
        $('#' + $('#currentSelectedLI').val()).find('.panel-body').css("height", ch_height);
        if ($('#' + $('#currentSelectedLI').val()).find('img').attr("id") == "|image|") {
            $('#' + $('#currentSelectedLI').val()).find('img').attr('save_width', "|" + $('#' + $('#currentSelectedLI').val()).attr('data-sizex') + "|");
            $('#' + $('#currentSelectedLI').val()).find('img').attr('save_height', "|" + $('#' + $('#currentSelectedLI').val()).attr('data-sizey') + "|");
        }
    });
    $('.gs-resize-handle').mouseenter(function () {
        var height = $('#' + $('#currentSelectedLI').val()).height();
        var ch_height = height - 50;
        $('#' + $('#currentSelectedLI').val()).find('.panel-body').css("height", ch_height);
        if ($('#' + $('#currentSelectedLI').val()).find('img').attr("id") == "|image|") {
            $('#' + $('#currentSelectedLI').val()).find('img').attr('save_width', "|" + $('#' + $('#currentSelectedLI').val()).attr('data-sizex') + "|");
            $('#' + $('#currentSelectedLI').val()).find('img').attr('save_height', "|" + $('#' + $('#currentSelectedLI').val()).attr('data-sizey') + "|");
        }
    }).mouseleave(function () {
        var height = $('#' + $('#currentSelectedLI').val()).height();
        var ch_height = height - 50;
        $('#' + $('#currentSelectedLI').val()).find('.panel-body').css("height", ch_height);
        if ($('#' + $('#currentSelectedLI').val()).find('img').attr("id") == "|image|") {
            $('#' + $('#currentSelectedLI').val()).find('img').attr('save_width', "|" + $('#' + $('#currentSelectedLI').val()).attr('data-sizex') + "|");
            $('#' + $('#currentSelectedLI').val()).find('img').attr('save_height', "|" + $('#' + $('#currentSelectedLI').val()).attr('data-sizey') + "|");
        }
    }).mousemove(function () {
        var height = $('#' + $('#currentSelectedLI').val()).height();
        var ch_height = height - 50;
        $('#' + $('#currentSelectedLI').val()).find('.panel-body').css("height", ch_height);
        if ($('#' + $('#currentSelectedLI').val()).find('img').attr("id") == "|image|") {
            $('#' + $('#currentSelectedLI').val()).find('img').attr('save_width', "|" + $('#' + $('#currentSelectedLI').val()).attr('data-sizex') + "|");
            $('#' + $('#currentSelectedLI').val()).find('img').attr('save_height', "|" + $('#' + $('#currentSelectedLI').val()).attr('data-sizey') + "|");
        }
    });
}
function saveSelection() {
    if (window.getSelection) {
        sel = window.getSelection();
        if (sel.getRangeAt && sel.rangeCount) {
            return sel.getRangeAt(0);
        }
    } else if (document.selection && document.selection.createRange) {
        return document.selection.createRange();
    }
    return null;
}

function restoreSelection(range) {
    if (range) {
        if (window.getSelection) {
            sel = window.getSelection();
            sel.removeAllRanges();
            sel.addRange(range);
        } else if (document.selection && range.select) {
            range.select();
        }
    }
}

function insertTextAtCursor(html) {
    sel = window.getSelection();
    if (sel.getRangeAt && sel.rangeCount) {
        range = sel.getRangeAt(0);
        range.deleteContents();
        var el = document.createElement("div");
        el.innerHTML = html;
        var frag = document.createDocumentFragment(), node, lastNode;
        while ((node = el.firstChild)) {
            lastNode = frag.appendChild(node);
        }
        range.insertNode(frag);

        if (lastNode) {
            range = range.cloneRange();
            range.setStartAfter(lastNode);
            range.collapse(true);
            sel.removeAllRanges();
            sel.addRange(range);
        }
    } else if (document.selection && document.selection.type != "Control") {
        document.selection.createRange().pasteHTML(html);
    }
}