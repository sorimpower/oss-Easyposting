marked.setOptions({
    renderer: new marked.Renderer(),
    gfm: true,
    tables: true,
    breaks: false,
    pedantic: false,
    sanitize: true,
    smartLists: true,
    smartypants: false
});
var selRange;
$(document).ready(function () {
    var output2 = $('.markdown_output2').text();
    $('#in').val(output2);

    $('#markdown-submit').click(function () {
        var result_html = $('#in').val();
        var result_visual = $('#out').html();
        $('.markdown_output').text(result_visual);
        $('.markdown_output2').text(result_html);
        $('.mime').text("mk");
    });

    $('#Media_File').on('show.bs.modal', function () {
        $('#media_data_File').val("");
    });
    $('#Media_Image').on('show.bs.modal', function () {
        $('#media_data_Image').val("");
    });
    $('#Media_Video').on('show.bs.modal', function () {
        $('#media_data_Video').val("");
    });
    $('#Media_Map').on('show.bs.modal', function () {
        $('#media_data_Map').val("");
    });

    //$('#media_File').click(function () {
    //    selRange = saveSelection();
    //});

    $('#Media_File').on('hidden.bs.modal', function () {
        var src = $('#media_data_File').val();
        if (src != "") {
            var array = src.split('|');
            var start = "[";
            var middle = "](";
            var end = array[0] + ")";
            var result = start + array[1] + middle + end;
            //restoreSelection(selRange);
            $('#in').focus();
            //insertTextAtCursor(result);
            $('#in').insertAtCaret("\n\n" + result);
            var val = document.getElementById('in').value;
            document.getElementById('out').innerHTML = marked(val);
        }
    });

    $('#Media_Image').on('hidden.bs.modal', function () {
        var src = $('#media_data_Image').val();
        if (src != "") {
            var start = "![](";
            var end = ")";
            var result = start + src + end;
            $('#in').focus();
            $('#in').insertAtCaret("\n\n" + result);
            var val = document.getElementById('in').value;
            document.getElementById('out').innerHTML = marked(val);
        }
    });

    $('#Media_Video').on('hidden.bs.modal', function () {
        var src = $('#media_data_Video').val();
        var array = src.split("|");
        if (src != "") {
            $('#in').focus();
            $('#in').insertAtCaret("\n\n" + array[1]);
            var val = document.getElementById('in').value;
            document.getElementById('out').innerHTML = marked(val);
        }
    });

    $('#media_Map').click(function () {
        document.getElementById('MapEmb').contentWindow.location.reload(true);
    });

    $('#Media_Map').on('hidden.bs.modal', function () {
        var src = $('#markdown_media_data_Map').val();
        var array = src.split('|');
        if (src != "") {
            $('#in').focus();
            $('#in').insertAtCaret("\n\n" + array[1]);
            var val = document.getElementById('in').value;
            document.getElementById('out').innerHTML = marked(val);
        }        
    });

    $('#preview').click(function () {
        if ($('#in').css("display") == "none") {
            $('#in').css("display", "inline-block");
            $('#out').css("display", "none");
            $('#media_File').removeAttr('disabled');
            $('#media_Image').removeAttr('disabled');
            $('#media_Video').removeAttr('disabled');
            $('#media_Map').removeAttr('disabled');
        }
        else if ($('#in').css("display") == "inline-block") {
            var val = document.getElementById('in').value;
            document.getElementById('out').innerHTML = marked(val);
            $('#in').css("display", "none");
            $('#out').css("display", "inline-block");
            $('#media_File').attr('disabled', "disabled");
            $('#media_Image').attr('disabled', "disabled");
            $('#media_Video').attr('disabled', "disabled");
            $('#media_Map').attr('disabled', "disabled");
        }
    });

    $('#in').css("width", $('#mk').width());
    $('#out').css("width", $('#mk').width());
    $(window).resize(function () {
        $('#in').css("width", $('#mk').width());
        $('#out').css("width", $('#mk').width());
    });


});

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

$.fn.insertAtCaret = function (text) {
    return this.each(function () {
        if (document.selection && this.tagName == 'TEXTAREA') {
            //IE textarea support
            this.focus();
            sel = document.selection.createRange();
            sel.text = text;
            this.focus();
        } else if (this.selectionStart || this.selectionStart == '0') {
            //MOZILLA/NETSCAPE support
            startPos = this.selectionStart;
            endPos = this.selectionEnd;
            scrollTop = this.scrollTop;
            this.value = this.value.substring(0, startPos) + text + this.value.substring(endPos, this.value.length);
            this.focus();
            this.selectionStart = startPos + text.length;
            this.selectionEnd = startPos + text.length;
            this.scrollTop = scrollTop;
        } else {
            // IE input[type=text] and other browsers
            this.value += text;
            this.focus();
            this.value = this.value;    // forces cursor to end
        }
    });
};