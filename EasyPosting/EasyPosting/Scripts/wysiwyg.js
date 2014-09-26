(function () {
    (function ($) {
        var selRange;
        var link_selRange;
        var methods;
        methods = {
            edit: function (isEditing) {
                return this.each(function () {
                    return $(this).attr("contentEditable", isEditing || false);
                });
            },
            save: function (callback) {
                return this.each(function () {
                    return callback($(this).attr('id'), $(this).html());
                });
            },            
            unlink: function () {
                return document.execCommand("unlink", false, null);
            },
            insertimage: function () {
                var urlPrompt;
                urlPrompt = prompt("Enter Image URL:", "http://");
                return document.execCommand("insertimage", false, urlPrompt);
            },
            formatblock: function (block) {
                return document.execCommand("formatblock", null, block);
            },
            init: function (opts) {
                var $toolbar, button, command, commands, excludes, font, font_list, fontnames, fontsize, fontsizes, fontinfos, font_info, finfo, group, groups, options, shortcuts, size_list, _i, _j, _k, _l, _m, _len, _len2, _len3, _len4, _len5;
                options = opts || {};
                groups = [
                  [
                    {
                        name: 'bold',
                        label: "<i class='fa fa-bold'></i>",
                        title: '진하게 (Ctrl+B)',
                        classname: 'toolbar_bold'
                    }, {
                        name: 'italic',
                        label: "<i class='fa fa-italic'></i>",
                        title: '기울임 (Ctrl+I)',
                        classname: 'toolbar_italic'
                    }, {
                        name: 'underline',
                        label: "<i class='fa fa-underline'></i>",
                        title: '밑줄 (Ctrl+U)',
                        classname: 'toolbar_underline'
                    }, {
                        name: 'strikethrough',
                        label: "<i class='fa fa-strikethrough'></i>",
                        title: '취소선',
                        classname: 'toolbar_strikethrough'
                    }, {
                        name: 'removeFormat',
                        label: "<i class='fa fa-eraser'></i>",
                        title: '형식 제거 (Ctrl+M)',
                        classname: 'toolbar_remove'
                    }
                  ], [
                    {
                        name: 'fontname',
                        label: "<i class='fa fa-font'></i><span class='caret'></span>",
                        title: '글꼴',
                        classname: 'toolbar_fontname dropdown-toggle',
                        dropdown: true
                    }
                  ], [
                    {
                        name: 'FontSize',
                        label: "<i class='fa fa-text-height'></i><span class='caret'></span>",
                        title: '글자크기',
                        classname: 'toolbar_fontsize dropdown-toggle',
                        dropdown: true
                    }
                  ], [
                    {
                        name: 'fontinfo',
                        label: "<i class='fa fa-header'></i><span class='caret'></span>",
                        title: '글 속성',
                        classname: 'toolbar_fontinfo dropdown-toggle',
                        dropdown: true
                    }
                  ], [
                    {
                        name: 'forecolor',
                        label: "<div style='color:#000000;'>A <span class='caret'></span></div>",
                        title: '글자색',
                        classname: 'toolbar_forecolor dropdown-toggle',
                        dropdown: true
                    }
                  ], [
                    {
                        name: 'backcolor',
                        label: "<div style='display:inline-block;width:15px;height:12px;background-color:#000000;'></div> <span class='caret'></span>",
                        title: '글 배경색',
                        classname: 'toolbar_bgcolor dropdown-toggle',
                        dropdown: true
                    }
                  ], [
                      {
                          name: 'table',
                          label: "<i class='fa fa-th'></i><span class='caret'></span>",
                          title: '표 만들기',
                          classname: 'toolbar_table dropdown-toggle',
                          dropdown: true
                      }
                  ], [
                    {
                        name: 'justifyleft',
                        label: "<i class='fa fa-align-left'></i>",
                        title: '왼쪽 정렬',
                        classname: 'toolbar_justifyleft'
                    }, {
                        name: 'justifycenter',
                        label: "<i class='fa fa-align-center'></i>",
                        title: '가운데 정렬',
                        classname: 'toolbar_justifycenter'
                    }, {
                        name: 'justifyright',
                        label: "<i class='fa fa-align-right'></i>",
                        title: '오른쪽 정렬',
                        classname: 'toolbar_justifyright'
                    }, {
                        name: 'justifyfull',
                        label: "<i class='fa fa-align-justify'></i>",
                        title: '양쪽 정렬',
                        classname: 'toolbar_justifyfull'
                    }
                  ], [
                     {
                         name: 'insertorderedlist',
                         label: "<i class='fa fa-list-ol'></i>",
                         title: '순서 리스트',
                         classname: 'toolbar_ol'
                     }, {
                         name: 'insertunorderedlist',
                         label: "<i class='fa fa-list-ul'></i>",
                         title: '비순서 리스트',
                         classname: 'toolbar_ul'
                     }, {
                         name: 'indent',
                         label: "<i class='fa fa-indent'></i>",
                         title: '들여쓰기',
                         classname: 'toolbar_indent'
                     }, {
                         name: 'outdent',
                         label: "<i class='fa fa-outdent'></i>",
                         title: '내어쓰기',
                         classname: 'toolbar_outdent'
                     }
                  ], [
                      {
                          name: '',
                          label: "<i class='fa fa-undo'></i>",
                          title: '실행 취소',
                          classname: 'toolbar_undo'
                      }, {
                          name: '',
                          label: "<i class='fa fa-repeat'></i>",
                          title: '다시 실행',
                          classname: 'toolbar_redo'
                      }
                  ], [
                    {
                        name: 'createlink',
                        label: "<i class='fa fa-link'></i>",
                        title: '하이퍼링크 (Ctrl+L)',
                        userinput: "yes",
                        classname: 'toolbar_link dropdown-toggle',
                        dropdown: true
                    }, {
                        name: 'unlink',
                        label: "<i class='fa fa-unlink'></i>",
                        title: '하이퍼링크 제거 (Ctrl+N)',
                        classname: 'toolbar_unlink'
                    }, {
                        name: 'insertimage',
                        label: "<i class='fa fa-picture-o'></i>",
                        title: '이미지 삽입 (Ctrl+G)',
                        userinput: "yes",
                        classname: 'toolbar_image'
                    }, {
                        name: 'blockquote',
                        label: "<i class='fa fa-comment'></i>",
                        title: '인용구 (Ctrl+Q)',
                        classname: 'toolbar_blockquote',
                        block: 'blockquote'
                    }, {
                        name: 'code',
                        label: '{&nbsp;}',
                        title: '코드 하이라이트',
                        classname: 'toolbar_code',
                        block: 'pre'
                    }, {
                        name: 'superscript',
                        label: '<i class="fa fa-superscript"></i>',
                        title: '위 첨자',
                        classname: 'toolbar_superscript'
                    }, {
                        name: 'subscript',
                        label: '<i class="fa fa-subscript"></i>',
                        title: '아래 첨자',
                        classname: 'toolbar_subscript'
                    }
                  ]
                ];
                if (options.toolbar_selector != null) {
                    $toolbar = $(options.toolbar_selector);
                } else {
                    $(this).before("<div id='editor-toolbar'></div>");
                    $toolbar = $('#editor-toolbar');
                }
                $toolbar.addClass('wysiwyg-toolbar');
                $toolbar.append("<div class='btn-toolbar'></div>");
                excludes = options.excludes || [];
                for (_i = 0, _len = groups.length; _i < _len; _i++) {
                    commands = groups[_i];
                    group = '';
                    for (_j = 0, _len2 = commands.length; _j < _len2; _j++) {
                        command = commands[_j];
                        if (jQuery.inArray(command.name, excludes) < 0) {
                            button = "<a href='#' class='btn btn-sm btn-white toolbar-btn toolbar-cmd " + command.classname + "' title='" + command.title + "' command='" + command.name + "'";
                            if (command.userinput != null) {
                                button += " userinput='" + command.userinput + "'";
                            }
                            if (command.block != null) {
                                button += " block='" + command.block + "'";
                            }
                            if (command.dropdown) {
                                button += " data-toggle='dropdown'";
                            }
                            button += ">" + command.label + "</a>";
                            group += button;
                        }
                    }
                    $('.btn-toolbar', $toolbar).append("<div class='btn-group'>" + group + "</div>");
                }
                $("[data-toggle='dropdown']").removeClass('toolbar-cmd');
                if (jQuery.inArray('fontname', excludes) < 0) {
                    fontnames = ["돋움", "바탕", "궁서", "Arial", "Arial Black", "Comic Sans MS", "Courier New", "Georgia", "Helvetica", "Sans Serif", "Tahoma", "Times New Roman", "Trebuchet MS", "Verdana"];
                    font_list = '';
                    for (_k = 0, _len3 = fontnames.length; _k < _len3; _k++) {
                        font = fontnames[_k];
                        font_list += "<li><a href='#' class='fontname-option' style='font-family:" + font + ";'>" + font + "</a></li>";
                    }
                    $('.toolbar_fontname').after("<ul class='dropdown-menu'>" + font_list + "</ul>");
                    $('.fontname-option').on('click', function () {
                        document.execCommand("fontname", false, $(this).text());
                        $(this).closest('.btn-group').removeClass('open');
                        return false;
                    });
                }
                if (jQuery.inArray('table', excludes) < 0) {
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

                    $('.toolbar_table').click(function () {
                        selRange = saveSelection();
                        $('#col').val("");
                        $('#row').val("");
                    });
                    $('.toolbar_table').after("<div class='dropdown-menu table-setting'><form role='form'><div class='form-group'><label for='col'>열 갯수</label><input type='text' class='form-control' id='col'></input></div><div class='form-group'><label for='row'>행 갯수</label><input type='text' class='form-control' id='row'></input></div><div class='btn btn-primary btn-xs' id='table_submit'>확인</div><div class='btn btn-default btn-xs' id='table_cancel'>취소</div></form></div>");
                    $('.table-setting').click(function (e) {
                        e.stopPropagation();
                    });
                    $('#table_submit').click(function () {
                        var col = $('#col').val();
                        var row = $('#row').val();
                        var table_start = "<table border='1' width='100px' border-collapse='collapse'><tbody>";
                        var table_end = "</tbody></table>";
                        var table = "";
                        var result = "";
                        result += table_start;

                        for (var i = 1; i <= row; i++) {
                            result += "<tr>";
                            for (var j = 1; j <= col; j++) {
                                result += "<td class='noCrsr'><p>&nbsp;</p></td>";
                            }
                            result += "</tr>";
                        }
                        result += table_end;

                        restoreSelection(selRange);
                        $('#' + $('#currentSelectedLI').val()).find('.clear').focus();
                        insertTextAtCursor(result);
                        var tb_width = $('#' + $('#currentSelectedLI').val()).find('table').width();
                        $('#' + $('#currentSelectedLI').val()).find('table').removeAttr('width');
                        $('#' + $('#currentSelectedLI').val()).find('table td').css('width', tb_width / col);

                        $(this).closest('.btn-group').removeClass('open');
                    });
                    $('#table_cancel').click(function () {
                        $(this).closest('.btn-group').removeClass('open');
                    });
                }
                if (jQuery.inArray('createlink', excludes) < 0) {
                    function saveSelection() {
                        if (window.getSelection) {
                            sel = window.getSelection();
                            if (sel.getRangeAt && sel.rangeCount) {
                                var ranges = [];
                                for (var i = 0, len = sel.rangeCount; i < len; ++i) {
                                    ranges.push(sel.getRangeAt(i));
                                }
                                return ranges;
                            }
                        } else if (document.selection && document.selection.createRange) {
                            return document.selection.createRange();
                        }
                        return null;
                    }

                    function restoreSelection(savedSel) {
                        if (savedSel) {
                            if (window.getSelection) {
                                sel = window.getSelection();
                                sel.removeAllRanges();
                                for (var i = 0, len = savedSel.length; i < len; ++i) {
                                    sel.addRange(savedSel[i]);
                                }
                            } else if (document.selection && savedSel.select) {
                                savedSel.select();
                            }
                        }
                    }
                    $('.toolbar_link').click(function () {
                        link_selRange = saveSelection();
                        $('#url').val("http://");
                    });
                    var url;                
                    $('.toolbar_link').after("<div class='dropdown-menu link-setting'><form role='form'><div class='form-group'><label for='url'>주소</label><input type='text' class='form-control' id='url' value='http://'></input></div><div class='btn btn-primary btn-xs' id='link_submit'>확인</div><div class='btn btn-default btn-xs' id='link_cancel'>취소</div></form></div>");
                    $('.link-setting').click(function (e) {
                        e.stopPropagation();
                    });
                    $('#link_submit').click(function () {
                        url = $('#url').val();
                        restoreSelection(link_selRange);
                        document.execCommand("createlink", false, url);
                        $(this).closest('.btn-group').removeClass('open');
                    });
                    $('#link_cancel').click(function () {
                        $(this).closest('.btn-group').removeClass('open');
                    });
                }
                if (jQuery.inArray('fontinfo', excludes) < 0) {
                    fontinfos = ["H1", "H2", "H3", "H4", "p"];
                    font_info = '';
                    for (_m = 0, _len5 = fontinfos.length; _m < _len5; _m++) {
                        finfo = fontinfos[_m];
                        font_info += "<li><a href='#' class='fontinfo-option'>" + "<" + finfo + ">" + finfo + "</" + finfo + ">" + "</a></li>";
                    }
                    $('.toolbar_fontinfo').after("<ul class='dropdown-menu'>" + font_info + "</ul>");
                    $('.fontinfo-option').on('click', function () {
                        document.execCommand("formatblock", null, [$(this).text()]);
                        $(this).closest('.btn-group').removeClass('open');
                        return false;
                    });
                }
                if (jQuery.inArray('FontSize', excludes) < 0) {
                    fontsizes = [
                      {
                          size: '가나다라',
                          point: 1
                      }, {
                          size: '가나다라',
                          point: 2
                      }, {
                          size: '가나다라',
                          point: 3
                      }, {
                          size: '가나다라',
                          point: 4
                      }, {
                          size: '가나다라',
                          point: 5
                      }, {
                          size: '가나다라',
                          point: 6
                      }, {
                          size: '가나다라',
                          point: 7
                      }
                    ];
                    size_list = '';
                    for (_l = 0, _len4 = fontsizes.length; _l < _len4; _l++) {
                        fontsize = fontsizes[_l];
                        size_list += "<li><a href='#' class='font-option fontsize-option' fontsize='" + fontsize.point + "'><font size='" + fontsize.point + "'>" + fontsize.size + "(" + fontsize.point + ")</font></a></li>";
                    }
                    $('.toolbar_fontsize').after("<ul class='dropdown-menu'>" + size_list + "</ul>");
                    $('a.fontsize-option').on('click', function () {
                        document.execCommand("FontSize", false, $(this).attr('fontsize'));
                        $(this).closest('.btn-group').removeClass('open');
                        return false;
                    });
                }

                if (jQuery.inArray('forecolor', excludes) < 0) {
                    $('a.toolbar_forecolor').after("<ul class='dropdown-menu colorpanel'><input type='text' class='form-control' id='forecolor-input' /><div id='forecolor-picker'></div></ul>");
                    $('.colorpanel').click(function (e) {
                        e.stopPropagation();
                    });
                    $('#forecolor-picker').farbtastic(function (color) {
                        $('#forecolor-input').val(color);
                        document.execCommand("forecolor", false, color);
                        return false;
                    });
                }
                if (jQuery.inArray('backcolor', excludes) < 0) {
                    $('a.toolbar_bgcolor').after("<ul class='dropdown-menu colorpanel'><input type='text' class='form-control' id='bgcolor-input' /><div id='bgcolor-picker'></div></ul>");
                    $('.colorpanel').click(function (e) {
                        e.stopPropagation();
                    });
                    $('#bgcolor-picker').farbtastic(function (color) {
                        $('#bgcolor-input').val(color);
                        document.execCommand("backcolor", false, color);
                        return false;
                    });
                }
                $(this).on('focus', function () {
                    var $this;
                    $this = $(this);
                    $this.data('before', $this.html());
                    return $this;
                }).on('blur keyup paste', function () {
                    var $this;
                    $this = $(this);
                    if ($this.data('before') !== $this.html()) {
                        $this.data('before', $this.html());
                        $this.trigger('change');
                    }
                    return $this;
                });
                $("a.toolbar-cmd").on('click', function () {
                    var ceNode, cmd, dummy, range;
                    cmd = $(this).attr('command');
                    if ($(this).attr('userinput') === 'yes') {
                        methods[cmd].apply(this);
                    } else if ($(this).attr('block')) {
                        methods['formatblock'].apply(this, ["<" + ($(this).attr('block')) + ">"]);
                    } else {
                        if ((cmd === 'justifyright') || (cmd === 'justifyleft') || (cmd === 'justifycenter') || (cmd === 'justifyfull')) {
                            try {
                                document.execCommand(cmd, false, null);
                            } catch (e) {
                                if (e && e.result === 2147500037) {
                                    range = window.getSelection().getRangeAt(0);
                                    dummy = document.createElement('br');
                                    ceNode = range.startContainer.parentNode;
                                    while ((ceNode != null) && ceNode.contentEditable !== 'true') {
                                        ceNode = ceNode.parentNode;
                                    }
                                    if (!ceNode) {
                                        throw 'not editable';
                                    }
                                    ceNode.insertBefore(dummy, ceNode.childNodes[0]);
                                    document.execCommand(cmd, false, null);
                                    dummy.parentNode.removeChild(dummy);
                                } else if (console && console.log) {
                                    console.log(e);
                                }
                            }
                        } else {
                            document.execCommand(cmd, false, null);
                        }
                    }
                    return false;
                });
                shortcuts = [
                  {
                      keys: 'Ctrl+l',
                      method: function () {
                          return methods.createlink.apply(this);
                      }
                  }, {
                      keys: 'Ctrl+n',
                      method: function () {
                          return methods.unlink.apply(this);
                      }
                  }, {
                      keys: 'Ctrl+b',
                      method: function () {
                          return document.execCommand('bold', false, null);
                      }
                  }, {
                      keys: 'Ctrl+i',
                      method: function () {
                          return document.execCommand('italic', false, null);
                      }
                  }, {
                      keys: 'Ctrl+m',
                      method: function () {
                          return document.execCommand("removeFormat", false, null);
                      }
                  }, {
                      keys: 'Ctrl+u',
                      method: function () {
                          return document.execCommand('underline', false, null);
                      }
                  }, {
                      keys: 'tab',
                      method: function () {
                          return document.execCommand('indent', false, null);
                      }
                  }, {
                      keys: 'Shift+tab',
                      method: function () {
                          return document.execCommand('outdent', false, null);
                      }
                  }
                ];
                $.each(shortcuts, function (index, elem) {
                    return shortcut.add(elem.keys, function () {
                        elem.method();
                        return false;
                    }, {
                        'type': 'keydown',
                        'propagate': false
                    });
                });
                return this.each(function () {
                    var $this, data, tooltip;
                    $this = $(this);
                    data = $this.data('wysiwyg');
                    tooltip = $('<div/>', {
                        text: $this.attr('title')
                    });
                    if (!data) {
                        return $(this).data('wysiwyg', {
                            target: $this,
                            tooltip: tooltip
                        });
                    }
                });
            }
        };
        return $.fn.wysiwyg = function (method) {
            if (methods[method]) {
                methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
            } else if (typeof method === 'object' || !method) {
                methods.init.apply(this, arguments);
            } else {
                $.error('Method ' + method + ' error');
            }
        };
    })(jQuery);
}).call(this);
