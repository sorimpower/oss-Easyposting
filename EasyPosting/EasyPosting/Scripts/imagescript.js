window.onload = function () {
    var clickOn = 1;
    var imageID = 1;
    var focusID = 1;
    var jcropAPI;
    var maxLength = 350;
    var maxNum = 10;
    var maxSize = 4;
    var ratioOn = 0;
    var image = new Array();
    var imagePath = new Array();
    var alive = new Array();
    var zidx = new Array();
    var clip = new Array();

    // 초기 설정
    if (typeof $('#avatar-upload-form') !== undefined) {
        $('#avatar-upload-form').ajaxForm({
            success: function (data) {
                if (data.success === true) {
                    for (var i = 0; i < data.nums; i++) {
                        imagePath[imageID] = data.fileName[i];
                        var offTop = ($('#editor').height() / 8) * (parseInt(((imageID + 1) % 4) / 2) + 1) + Math.floor((Math.random() * 100) + 1);
                        var offLeft = ($('#editor').width() / 8) * (parseInt((imageID % 4) / 2) + 1) + Math.floor((Math.random() * 100) + 1);
                        imageView(imageID, data.fileName[i], data.width[i], data.height[i], offLeft, offTop, 1);
                        imageID++;
                    }
                    recodeUndo();
                }
            }
        });
    }
    $('input[type=file]').bootstrapFileInput({}); // 파일 인풋 디자인 설정
    $('#editor').css('height', $(window).height * 1.5); // 에디터 크기 설정
    $('html').css("overflow", "hidden");  // 스크롤 금지 * $('html').css("overflow", "hidden");   
    App.init();

    // 브라우저 설정
    var agent = {
        isMac: navigator.appVersion.indexOf('Mac') > -1,
        isMSIE: navigator.userAgent.indexOf('MSIE') > -1 || navigator.userAgent.indexOf('Trident') > -1,
        isFF: navigator.userAgent.indexOf('Firefox') > -1
    };

    // 블록지정 금지
    $(document).bind("mousemove", function () { return false; });
    $(document).mouseup(function () {
        $(document).bind("mousemove", function () { return false; });
        if (cropflag) cropflag = 0;
    });

    //에디터 클릭 이벤트
    $('#editor').click(function () {
        if (clickOn) {
            clickOn = 0;
            return;
        }
        else if (cropping == 0) {
            hidehandle();
            focusID = 0;
            focusDiagram = 0;
            if (textOn) deleteTextControl(0);
        }
    });
    $('#editor').mousedown(function () {
        if (cropping == 1) {
            image_crop();
            clickOn = 1;
        }
    });

    // 네비바 클릭이벤트
    $('.navbar').mouseup(function () {
        if (textOn) deleteTextControl(1);
        if (cropflag) return;
        else if (cropping == 1) {
            image_crop();
        }
    });

    // 이미지 삽입 시 실행 함수
    $('input[type="file"]').change(function () {
        var imgfiles = document.getElementById("upFile").files;
        var allSize = 0;
        if (this.value == '') return;

        // 용량 제한
        for (var i = 0; i < imgfiles.length; i++) allSize += imgfiles[i].size;
        if (allSize > maxSize * 1048576) {
            $.gritter.add({
                title: "이미지 총 " + maxSize + "MB를 초과할 수 없습니다."
            });
            this.value = '';
            return;
        }

        for (var i = 0; i < imgfiles.length; i++) {
            var strArray = imgfiles[i].name.split('.');
            if (this.value == '' || strArray.length == 1) return;
            var ext = strArray[strArray.length - 1];
            if (imageID + imgfiles.length > maxNum) {
                $.gritter.add({
                    title: "이미지는 10개 이상 넣을 수 없습니다."
                });
                this.value = '';
                return;
            }
            else {
                switch (ext) {
                    case 'jpg':
                    case 'JPG':
                    case 'jpeg':
                    case 'png':
                    case 'PNG':
                        $('#uploadButton').attr('disabled', false);
                        break;
                    default:
                        $.gritter.add({
                            title: "이미지 타입이 아닙니다."
                        });
                        this.value = '';
                        break;
                        return;
                }
            }
        }
        // 이미지 업로드
        $('#avatar-upload-form').submit();
        this.value = '';
    });

    //스토리지 갱신
    $('#widget-AddWidget-media').click(function () {
        document.getElementById('ImageEmbed').contentWindow.location.reload(true);
    });

    // 스토리지 모달 종료시 업로드 실행
    $('#widgetMedia_Image').on('hidden.bs.modal', function () {
        var storageImgPath = $('#media_data_Image').attr('value');
        $.ajax({
            type: "POST",
            url: "/Image/SaveStorageImage",
            traditional: true,
            data: {
                fileName: storageImgPath
            }
        }).done(function (data) {
            if (data.success === true) {
                $('#media_data_Image').attr('value', '');
                imagePath[imageID] = data.filePath;
                var offTop = ($('#editor').height() / 8) * (parseInt(((imageID + 1) % 4) / 2) + 1) + Math.floor((Math.random() * 100) + 1);
                var offLeft = ($('#editor').width() / 8) * (parseInt((imageID % 4) / 2) + 1) + Math.floor((Math.random() * 100) + 1);
                imageView(imageID, data.filePath, data.width, data.height, offLeft, offTop, 1);
                imageID++;
            }
        }).fail(function (e) {
            $.gritter.add({
                title: "이미지 업로드에 실패하였습니다."
            });
        });
    });

    // 서버 이미지 브라우저에 띄우기
    function imageView(id, filePath, width, height, left, top, upOn) {
        $('#editor').append('<div id ="d' + id + '" class="imgDiv"><img id ="' + id + '" src="' + filePath + "?" + new Date().getTime() + '"></img></div>');
        $("#upFile").val('');
        alive[id] = 1;

        // 이미지 속성   
        $('#d' + id).css('z-index', id);
        $('.imgDiv').css('display', 'inline-block');
        $('.imgDiv').css('position', 'absolute');
        $('#d' + id).offset({ top: top, left: left });
        focusID = zidx[id] = id;

        // 이미지 사이즈 설정
        image[id] = document.getElementById(id);
        if (upOn) {
            if (width > height) {
                var k = maxLength / width;
                height = parseInt(k * height, 10);
                width = maxLength;
            }
            else {
                var k = maxLength / height;
                width = parseInt(k * width, 10);
                height = maxLength;
            }
        }
        image[id].width = width;
        image[id].height = height;

        // 이벤트 설정
        $('.imgDiv').draggable({ cursor: "move", stop: function () { recodeUndo(); } });                                 // 드래그 허용 / 범위 지정 = containment: 'parent'
        if (ratioOn) $('#' + id).resizable({ handles: "n,s,e,w,ne,nw,se,sw", stop: function () { recodeUndo(); }, aspectRatio: true });	// 리사이즈 허용
        else $('#' + id).resizable({ handles: "n,s,e,w,ne,nw,se,sw", stop: function () { recodeUndo(); } });
        hidehandle();
        $('#d' + id).removeClass("ui-resizable-autohide");						// 핸들 숨김 제거

        // 이미지 마우스 다운 이벤트
        $('#d' + id).mousedown(function () {
            var tempZidx;
            clickOn = 1;
            if (cropping == 0) {
                // 클릭 시 맨 위로 사진 올라옴.
                if ($(this).css("z-index") != imageID - 1) {
                    for (var i = 1; i < imageID; i++) {
                        tempZidx = $('#d' + i).css("z-index");
                        if (tempZidx > parseInt($(this).css("z-index"))) {
                            $('#d' + i).css('z-index', tempZidx - 1);
                            zidx[tempZidx - 1] = i;
                        }
                    }
                    $(this).css('z-index', imageID - 1);
                    zidx[imageID - 1] = this.id[1];
                    if (this.id[2]) zidx[imageID - 1] = this.id[1] + this.id[2];
                }
                // 포커스 설정
                hidehandle();
                if (textOn) deleteTextControl(0);
                $(this).removeClass("ui-resizable-autohide");							 // 핸들 표시
                focusID = this.id[1];
                if (this.id[2]) focusID = this.id[1] + this.id[2];
                focusDiagram = 0;
            }
        });
    }

    // 핸들 숨김
    function hidehandle() {
        for (var i = 1; i < imageID; i++) {
            $('#d' + i).addClass("ui-resizable-autohide");
        }
        for (var i = 1; i <= diagramID; i++) {
            $('#ad' + i).addClass("ui-resizable-autohide");
        }
    }

    //비율 버튼
    $('#ratio').click(function () {
        $(this).toggleClass("btn-info btn-link");
        if (ratioOn) {
            ratioOn = 0;
            for (var i = 1; i < imageID; i++) {
                if (alive[i]) {
                    $('#' + i).resizable('destroy');
                    $('#d' + i).removeClass("ui-resizable-autohide");
                    $('#' + i).resizable({ handles: "n,s,e,w,ne,nw,se,sw", stop: function () { recodeUndo(); } });
                }
            }
            for (var i = 1; i <= diagramID; i++) {
                if (aliveD[i]) {
                    $('#a' + i).resizable('destroy');
                    $('#ad' + i).removeClass("ui-resizable-autohide");
                    $('#a' + i).resizable({ handles: "ne,nw,se,sw", stop: function () { recodeUndo(); } });
                }
            }
        }
        else {
            ratioOn = 1;
            for (var i = 1; i < imageID; i++) {
                if (alive[i]) {
                    $('#' + i).resizable('destroy');
                    $('#d' + i).removeClass("ui-resizable-autohide");
                    $('#' + i).resizable({ handles: "n,s,e,w,ne,nw,se,sw", stop: function () { recodeUndo(); }, aspectRatio: true });
                }
            }
            for (var i = 1; i <= diagramID; i++) {
                if (aliveD[i]) {
                    $('#a' + i).resizable('destroy');
                    $('#ad' + i).removeClass("ui-resizable-autohide");
                    $('#a' + i).resizable({ handles: "ne,nw,se,sw", stop: function () { recodeUndo(); }, aspectRatio: true });
                }
            }
        }
        hidehandle();
    });

    //미리보기 modal버튼
    $('#preview').click(function () {
        var flag = drawPreviewCanvas();
        if (flag) $('#previewModal').modal('show');
    });
    function drawPreviewCanvas() {
        var left = 10000, top = 10000, right = 0, bottom = 0, k_max, on = 0;
        // 크기 탐색
        for (var i = 1; i < imageID; i++) {
            if (alive[i]) {
                var offset = $('#' + i).offset();
                if (offset.left < left) left = offset.left;
                if (offset.top < top) top = offset.top;
                if (offset.left + image[i].clientWidth > right) right = offset.left + image[i].clientWidth;
                if (offset.top + image[i].clientHeight > bottom) bottom = offset.top + image[i].clientHeight;
                on = 1;
            }
        }
        for (var i = 1; i <= diagramID; i++) {
            if (aliveD[i]) {
                var img = document.getElementById('a' + i);
                var offset = $('#a' + i).offset();
                if (offset.left < left) left = offset.left;
                if (offset.top < top) top = offset.top;
                if (offset.left + img.clientWidth > right) right = offset.left + img.clientWidth;
                if (offset.top + img.clientHeight > bottom) bottom = offset.top + img.clientHeight;
                on = 1;
            }
        }
        for (var i = 1; i < textID; i++) {
            if (aliveT[i]) {
                var img = document.getElementById('t' + i);
                var offset = $('#t' + i).offset();
                if (offset.left < left) left = offset.left;
                if (offset.top < top) top = offset.top;
                if (offset.left + img.clientWidth > right) right = offset.left + img.clientWidth;
                if (offset.top + img.clientHeight > bottom) bottom = offset.top + img.clientHeight;
                on = 1;
            }
        }

        // 이미지가 있으면 그리기
        if (on) {
            var maincanvas = document.getElementById('result');
            var context = maincanvas.getContext('2d');
            if (right - left > bottom - top) {
                k_max = 700 / (right - left);
            }
            else {
                k_max = 700 / (bottom - top);
            }
            maincanvas.width = k_max * (right - left) + 2 * c_gap;
            maincanvas.height = k_max * (bottom - top) + 2 * c_gap;
            $('#preview-body').css('height', maincanvas.height + 50);
            if (backColor != "") {
                context.fillStyle = backColor;
                context.fillRect(0, 0, maincanvas.width, maincanvas.height);
            }

            // 캔버스에 그리기
            for (var i = 1; i < imageID; i++) {
                if (alive[zidx[i]]) {                           // 사진
                    var offset = $('#' + zidx[i]).offset();
                    var img = document.getElementById(zidx[i]);
                    context.drawImage(img, c_gap + k_max * (offset.left - left), c_gap + k_max * (offset.top - top),
                        k_max * image[zidx[i]].clientWidth, k_max * image[zidx[i]].clientHeight);
                }
            }
            for (var i = 1; i <= diagramID; i++) {
                if (aliveD[i] == 1) {                          // 화살표
                    var offset = $('#a' + i).offset();
                    var img = document.getElementById('a' + i);
                    context.drawImage(img, c_gap + k_max * (offset.left - left), c_gap + k_max * (offset.top - top),
                        k_max * img.clientWidth, k_max * img.clientHeight);
                }
                else if (aliveD[i] == 2) {                      // 사각형
                    var offset = $('#a' + i).offset();
                    var img = document.getElementById('a' + i);
                    context.beginPath();
                    context.lineWidth = "3";
                    context.strokeStyle = "#f33";
                    context.rect(c_gap + k_max * (offset.left - left), c_gap + k_max * (offset.top - top),
                         k_max * img.clientWidth, k_max * img.clientHeight);
                    context.stroke();
                }
            }

            var tempcanvas = document.getElementById('temp-canvas');
            var ctx = tempcanvas.getContext('2d');

            $('#font-testing').show();     // 폰트 테스트 테그 활성화
            $('#temp-canvas').show();       // 폰트 캔버스 활성화
            for (var i = 1; i < textID; i++) {
                if (aliveT[i] == 1) {
                    var offset = $('#t' + i).offset();
                    tempcanvas.width = $('#text-area' + i).width();
                    tempcanvas.height = $('#text-area' + i).height();
                    printTextBox(ctx, i);
                    var img = new Image();
                    var imgUrl = tempcanvas.toDataURL("image/png");
                    img.src = imgUrl;
                    context.drawImage(img, c_gap + k_max * (offset.left - left), c_gap + k_max * (offset.top - top),
                        k_max * tempcanvas.width, k_max * tempcanvas.height);
                }
            }
            $('#font-testing').hide();     // 폰트 테스트 테그 숨기기
            $('#temp-canvas').hide();      // 폰트 캔버스 숨기기
        }
        return on;
    }

    // 저장 버튼
    $('#save1').click(function () {
        var on = drawPreviewCanvas();
        if (on) {
            // ajax를 통해 저장
            var maincanvas = document.getElementById('result');
            var imgUrl = maincanvas.toDataURL("image/png");

            imgUrl = imgUrl.replace('data:image/png;base64,', '');
            $.ajax({
                type: "POST",
                url: "/Image/SaveByteArrayAsImage",
                traditional: true,
                data: {
                    filename: "test",
                    base64String: imgUrl
                }
            }).done(function (data) {
                if (data.success === true) {
                    $.gritter.add({
                        title: "스토리지 저장에 성공하였습니다."
                    });
                }
            }).fail(function (e) {
                $.gritter.add({
                    title: "스토리지 저장에 실패하였습니다."
                });
            });
        }
    });
    $('#save2').click(function () {
        // ajax를 통해 저장
        var maincanvas = document.getElementById('result');
        var imgUrl = maincanvas.toDataURL("image/png");

        imgUrl = imgUrl.replace('data:image/png;base64,', '');
        $.ajax({
            type: "POST",
            url: "/Image/SaveByteArrayAsImage",
            traditional: true,
            data: {
                filename: "test",
                base64String: imgUrl
            }
        }).done(function (data) {
            if (data.success === true) {
                $.gritter.add({
                    title: "스토리지 저장에 성공하였습니다."
                });
                $('#previewModal').modal('hide');
            }
        }).fail(function (e) {
            $.gritter.add({
                title: "스토리지 저장에 실패하였습니다."
            });
        });
    });

    // deletekey 이벤트
    $(document).keydown(function (e) {
        if (!agent.isMSIE) initfont();
        //deletekey
        if (e.keyCode == 46) {
            $('#d' + focusID).remove();
            if (focusID != 0) maxNum++;
            alive[focusID] = 0;
            focusID = 0;
            $('#ad' + focusDiagram).remove();
            aliveD[focusDiagram] = 0;
            focusDiagram = 0;
            recodeUndo();
        }
    });
    // deletekey (이미지 제거)
    $('#delete').click(function () {
        $('#d' + focusID).remove();
        if (focusID != 0) maxNum++;
        alive[focusID] = 0;
        focusID = 0;
        $('#ad' + focusDiagram).remove();
        aliveD[focusDiagram] = 0;
        focusDiagram = 0;
        $('#dt' + focusText).remove();
        if (focusText != 0) maxTextNum++;
        aliveT[focusText] = 0;
        focusText = 0;
    });

    // up & down 버튼
    $('#up').click(function () {
        var z = $('#d' + focusID).css('z-index');
        var i = z;
        do {
            i++;
        } while (alive[zidx[i]] == 0 && i != imageID);
        if (i != imageID) {
            $('#d' + focusID).css('z-index', i);
            $('#d' + zidx[i]).css('z-index', z);
            zidx[z] = zidx[i];
            zidx[i] = focusID;
        }
    });
    $('#down').click(function () {
        var z = $('#d' + focusID).css('z-index');
        var i = z;
        do {
            i--;
        } while (alive[zidx[i]] == 0 && i != 0);
        if (i != 0) {
            $('#d' + focusID).css('z-index', i);
            $('#d' + zidx[i]).css('z-index', z);
            zidx[z] = zidx[i];
            zidx[i] = focusID;
        }
    });

    //-------------------------------- crop 부분 -------------------------------------//
    var cropping = 0;
    var cropflag = 0;
    var imgMouseDown = 0;

    // crop 버튼(토글)
    $('#crop').mousedown(function () {
        if (focusID == 0 || imageID == 1 || cropping == 1) return;
        // 크롭영역 작업
        if (cropping == 0) {
            $(this).toggleClass("btn-info btn-link");
            $('#' + focusID).fadeTo(0, 0.7);
            cropping = 1;
            cropflag = 1;
            for (var i = 1; i < imageID; i++) {
                $('#' + i).resizable('disable');
            }
            $('.imgDiv').draggable('disable');
            $('#' + focusID).Jcrop({ onChange: updatePane }, function () { jcropAPI = this; });
            //자르기 방지 이벤트
            $('.jcrop-holder').click(function () {
                clickOn = 1;
            });
        }
        else {
            image_crop();
        }
    });

    // 이미지 크롭 실행
    function image_crop() {
        if (cropping) {
            $('#crop').toggleClass("btn-info btn-link");
            $('#' + focusID).fadeTo(0, 1);
            cropping = 0;
            jcropAPI.destroy();
            if (clip[2] > 0 && clip[1] > 0) {
                saveclipImage();
            }
            for (var i = 1; i < imageID; i++) {
                $('#' + i).resizable('enable');
            }
            $('.imgDiv').draggable('enable');
        }
    }


    //이미지 crop update
    function updatePane(c) {
        clip[0] = Math.round(c.y);
        clip[1] = Math.round(c.w);
        clip[2] = Math.round(c.h);
        clip[3] = Math.round(c.x);
    }

    // crop 이미지 저장
    function saveclipImage() {
        var img = $('#' + focusID);
        $.ajax({
            type: "POST",
            url: "/Image/SaveClipImage",
            traditional: true,
            data: {
                t: clip[0] + 'px',
                l: clip[3] + 'px',
                h: img.css('height'),
                w: img.css('width'),
                ah: clip[2] + 'px',
                aw: clip[1] + 'px',
                fileName: imagePath[focusID]
            }
        }).done(function (data) {
            if (data.success === true) {
                imagePath[focusID] = data.filePath;
                imageReload();
                recodeUndo();
            } else {
            }
        }).fail(function (e) {
        });
    }

    // crop한 이미지 리로드
    function imageReload() {
        var tempDiv = $('#d' + focusID);
        var offset = $('#' + focusID).offset();

        $('#' + focusID).resizable('destroy');
        $('#' + focusID).remove();

        tempDiv.append('<img id ="' + focusID + '" src="' + imagePath[focusID] + "?" + new Date().getTime() + '"></img>');
        image[focusID] = document.getElementById(focusID);
        image[focusID].width = clip[1];
        image[focusID].height = clip[2];
        tempDiv.offset({ top: offset.top + clip[0], left: offset.left + clip[3] });
        if (ratioOn) $('#' + focusID).resizable({ handles: "n,s,e,w,ne,nw,se,sw", stop: function () { recodeUndo(); }, aspectRatio: true });	// 리사이즈 허용
        else $('#' + focusID).resizable({ handles: "n,s,e,w,ne,nw,se,sw", stop: function () { recodeUndo(); } });
        clip[0] = clip[1] = clip[2] = clip[3] = 0;
    }
    //-------------------------------- crop 끝  --------------------------------------//

    //------------------------------  효과 부분  -------------------------------------//

    var effectNum = 8;
    var frameNum = 12;
    var effectName;

    // 효과 버튼(섬네일 생성 및 view)
    $('#effect').click(function () {
        if (focusID == 0) return;
        var tempname = imagePath[focusID].replace(/^.*[\\\/]/, ''); // 경로에서 파일명 추출 정규식
        effectName = '';
        $.ajax({
            type: "POST",
            url: "/Image/SavethumnailImages",
            traditional: true,
            data: {
                fileName: imagePath[focusID]
            }
        }).done(function (data) {
            if (data.success === true) {
                for (var i = 1; i <= effectNum; i++) {
                    var tempsrc = $('#E' + i).attr('src');
                    var strArray = tempsrc.split('-');
                    $('#E' + i).attr('src', strArray[0] + '-' + tempname + "?" + new Date().getTime());
                }
                $('#effectModal').modal('show');
            } else {
            }
        }).fail(function (e) {
        });
    });

    // 섬네일 제거 (성공 시 이미지 효과 적용)
    function removeThumnails(filepath) {
        $.ajax({
            type: "POST",
            url: "/Image/DeletethumnailImages",
            traditional: true,
            data: {
                fileName: filepath
            }
        }).done(function (data) {
            if (data.success === true) {
                if (effectName) affectImage(effectName);    // 효과 버튼 클릭 시 효과 적용
            } else {
            }
        }).fail(function (e) {
        });
    }

    // 효과 이미지 마우스 이벤트
    $(".imgbtnEffect").mouseenter(function () {
        $(this).toggleClass("btn-primary btn-link");
    });
    $(".imgbtnEffect").mouseleave(function () {
        $(this).toggleClass("btn-primary btn-link");
    });

    // 모달 종료 이벤트
    $('#effectModal').on('hidden.bs.modal', function () {
        removeThumnails(imagePath[focusID]);
    })

    // ajax를 통해 원본 이미지에 효과 적용
    function affectImage(name) {
        clip[0] = 0;
        clip[1] = image[focusID].clientWidth;
        clip[2] = image[focusID].clientHeight;
        clip[3] = 0;
        $.ajax({
            type: "POST",
            url: "/Image/SaveaffectedImage",
            traditional: true,
            data: {
                effect: name,
                fileName: imagePath[focusID]
            }
        }).done(function (data) {
            if (data.success === true) {
                imagePath[focusID] = data.filePath;
                imageReload();
                recodeUndo();
            } else {
            }
        }).fail(function (e) {
        });
    }

    //밝게 버튼
    $('#Eb1').click(function () {
        if (focusID == 0) return;
        else {
            effectName = "brightly";
            $('#effectModal').modal('hide');
        }
    });
    //어둡게 버튼
    $('#Eb2').click(function () {
        if (focusID == 0) return;
        else {
            effectName = "darkly";
            $('#effectModal').modal('hide');
        }
    });
    //흑백 버튼
    $('#Eb3').click(function () {
        if (focusID == 0) return;
        else {
            effectName = "gray";
            $('#effectModal').modal('hide');
        }
    });
    //흐리게 버튼
    $('#Eb4').click(function () {
        if (focusID == 0) return;
        else {
            effectName = "smoothly";
            $('#effectModal').modal('hide');
        }
    });
    //흐리게 버튼
    $('#Eb5').click(function () {
        if (focusID == 0) return;
        else {
            effectName = "clearly";
            $('#effectModal').modal('hide');
        }
    });
    //카툰 버튼
    $('#Eb6').click(function () {
        if (focusID == 0) return;
        else {
            effectName = "cartoonize";
            $('#effectModal').modal('hide');
        }
    });
    //좌우반전
    $('#Eb7').click(function () {
        if (focusID == 0) return;
        else {
            effectName = "mirrorx";
            $('#effectModal').modal('hide');
        }
    });
    //상하반전
    $('#Eb8').click(function () {
        if (focusID == 0) return;
        else {
            effectName = "mirrory";
            $('#effectModal').modal('hide');
        }
    });

    // 액자 버튼 클릭
    $("#frame").click(function () {
        if (focusID == 0) return;
        var tempname = imagePath[focusID].replace(/^.*[\\\/]/, ''); // 경로에서 파일명 추출 정규식
        effectName = '';
        $.ajax({
            type: "POST",
            url: "/Image/SaveFramethumnailImages",
            traditional: true,
            data: {
                fileName: imagePath[focusID]
            }
        }).done(function (data) {
            if (data.success === true) {
                for (var i = 1; i <= frameNum; i++) {
                    var tempsrc = $('#F' + i).attr('src');
                    var strArray = tempsrc.split('-');
                    $('#F' + i).attr('src', strArray[0] + '-' + tempname + "?" + new Date().getTime());
                }
                $('#frameModal').modal('show');
            } else {
            }
        }).fail(function (e) {
        });
    });

    // 프레임 섬네일 제거 (성공 시 이미지 효과 적용)
    function removeFrameThumnails(filepath) {
        $.ajax({
            type: "POST",
            url: "/Image/DeleteFramethumnailImages",
            traditional: true,
            data: {
                fileName: filepath
            }
        }).done(function (data) {
            if (data.success === true) {
                if (effectName) affectImage(effectName);    // 효과 버튼 클릭 시 효과 적용
            } else {
            }
        }).fail(function (e) {
        });
    }

    // 프레임 모달 종료 이벤트
    $('#frameModal').on('hidden.bs.modal', function () {
        removeFrameThumnails(imagePath[focusID]);
    })

    // 액자 내부 버튼 
    $('#Fb1').click(function () {
        if (focusID == 0) return;
        else {
            effectName = "frame1";
            $('#frameModal').modal('hide');
        }
    });
    $('#Fb2').click(function () {
        if (focusID == 0) return;
        else {
            effectName = "frame2";
            $('#frameModal').modal('hide');
        }
    });
    $('#Fb3').click(function () {
        if (focusID == 0) return;
        else {
            effectName = "frame3";
            $('#frameModal').modal('hide');
        }
    });
    $('#Fb4').click(function () {
        if (focusID == 0) return;
        else {
            effectName = "frame4";
            $('#frameModal').modal('hide');
        }
    });
    $('#Fb5').click(function () {
        if (focusID == 0) return;
        else {
            effectName = "frame5";
            $('#frameModal').modal('hide');
        }
    });
    $('#Fb6').click(function () {
        if (focusID == 0) return;
        else {
            effectName = "frame6";
            $('#frameModal').modal('hide');
        }
    });
    $('#Fb7').click(function () {
        if (focusID == 0) return;
        else {
            effectName = "frame7";
            $('#frameModal').modal('hide');
        }
    });
    $('#Fb8').click(function () {
        if (focusID == 0) return;
        else {
            effectName = "frame8";
            $('#frameModal').modal('hide');
        }
    });
    $('#Fb9').click(function () {
        if (focusID == 0) return;
        else {
            effectName = "frame9";
            $('#frameModal').modal('hide');
        }
    });
    $('#Fb10').click(function () {
        if (focusID == 0) return;
        else {
            effectName = "polaroid1";
            $('#frameModal').modal('hide');
        }
    });
    $('#Fb11').click(function () {
        if (focusID == 0) return;
        else {
            effectName = "polaroid2";
            $('#frameModal').modal('hide');
        }
    });
    $('#Fb12').click(function () {
        if (focusID == 0) return;
        else {
            effectName = "polaroid3";
            $('#frameModal').modal('hide');
        }
    });

    //-------------------------------  효과 끝  --------------------------------------//

    //------------------------------  도형 부분  -------------------------------------//
    var diagramID = 0;
    var diagramName = new Array();
    var focusDiagram = 0;
    var dragFlag = 0;
    var dragStart = 0;
    var aliveD = new Array();
    var x1, x2, y1, y2;
    var ml, mt, mw, mh;

    // 초기 실행
    $('#dragview').hide();

    // 마우스 움직일 때 실행
    function mouseDraw() {
        dragFlag = 1;
        $('#editor').css('cursor', 'crosshair');
        $('#editor').fadeTo(0, 0.5);
        for (var i = 1; i < imageID; i++) {
            $('#' + i).resizable('disable');
        }
        $('.imgDiv').draggable('disable');
        for (var i = 1; i <= diagramID; i++) {
            $('#a' + i).resizable('disable');
            $('#ad' + i).draggable('disable');
        }
    }

    // 정상 상태로 변함
    function mouseRelease() {
        dragFlag = 0;
        dragStart = 0;
        $('#editor').fadeTo(0, 1);
        $('#editor').css('cursor', '');
        $('#dragview').hide();
        for (var i = 1; i < imageID; i++) {
            $('#' + i).resizable('enable');
        }
        $('.imgDiv').draggable('enable');
        for (var i = 1; i <= diagramID; i++) {
            $('#a' + i).resizable('enable');
            $('#ad' + i).draggable('enable');
        }
    }

    // 도형 설정
    function setDiagram(id) {
        $('#ad' + id).css('display', 'inline-block');
        $('#ad' + id).draggable({ cursor: "move", stop: function () { recodeUndo(); } });
        if (ratioOn) $('#a' + id).resizable({ handles: "ne,nw,se,sw", stop: function () { recodeUndo(); }, aspectRatio: true });
        else $('#a' + id).resizable({ handles: "ne,nw,se,sw", stop: function () { recodeUndo(); } });
        hidehandle();
        focusID = 0;
        focusText = 0;
        focusDiagram = id;
        $('#ad' + id).removeClass("ui-resizable-autohide");
        $('#ad' + id).mousedown(function () {
            clickOn = 1;
            hidehandle();
            if (textOn) deleteTextControl(0);
            $(this).removeClass("ui-resizable-autohide");
            focusID = 0;
            focusText = 0;
            focusDiagram = this.id[2];
            if (this.id[3]) focusDiagram = this.id[2] + this.id[3];
        });
    }

    // 도형 넣기
    function diagramView(id, dName, iw, ih, il, it) {
        var offset = document.getElementById('dragview');
        $('.body-content').css('width');

        if (dName == "rect") {
            aliveD[id] = 2;
            $('#editor').append('<div id ="ad' + id + '" style="z-index:800;"><div id="a' + id +
            '" style="border:3px solid #f33;"></div></div>');
            $('#a' + id).css('width', iw + 'px');
            $('#a' + id).css('height', ih + 'px');
            $('#ad' + id).css('position', 'absolute');
            $('#ad' + id).offset({ top: it, left: il });
            setDiagram(id);
        }
        else {
            aliveD[id] = 1;
            $('#editor').append('<div id ="ad' + id + '" style="z-index:800;"><img id="a' + id +
              '" src="/Resource/' + dName + '.png"></div>');
            $('#a' + id).css('width', iw + 'px');
            $('#a' + id).css('height', ih + 'px');
            $('#ad' + id).css('position', 'absolute');
            $('#ad' + id).offset({ top: it, left: il });
            setDiagram(id);
        }
    }

    // 도형 드래그 이벤트 설정
    $('.body-content').mousedown(function (e) {
        if (dragFlag) {
            x1 = e.clientX;
            y1 = e.clientY;
            dragStart = 1;
            clickOn = 1;
        }
    });
    $('.body-content').mousemove(function (e) {
        if (dragStart) {
            $('#dragview').show();
            x2 = e.clientX;
            y2 = e.clientY;
            ml = (x1 < x2) ? x1 : x2;
            mw = (x1 == ml) ? x2 - x1 : x1 - x2;
            mt = (y1 < y2) ? y1 : y2;
            mh = (y1 == mt) ? y2 - y1 : y1 - y2;
            $('#dragview').css('width', mw + 'px');
            $('#dragview').css('height', mh + 'px');
            $('#dragview').offset({ top: mt, left: ml });
        }
    });
    $('.body-content').mouseleave(function () {
        if (dragStart) {
            mouseRelease();
        }
    })
    $('.body-content').mouseup(function () {
        if (dragStart) {
            diagramID++;
            diagramView(diagramID, diagramName[diagramID], mw, mh, ml, mt);
            mouseRelease();
            recodeUndo();
        }
    });

    // 메뉴 도형 버튼 리스트
    $("#red_box").click(function () {
        mouseDraw();
        diagramName[diagramID + 1] = "rect";
    });
    $("#arrow_left").click(function () {
        mouseDraw();
        diagramName[diagramID + 1] = "image_arrow_left";
    });
    $("#arrow_right").click(function () {
        mouseDraw();
        diagramName[diagramID + 1] = "image_arrow_right";
    });
    $("#arrow_top").click(function () {
        mouseDraw();
        diagramName[diagramID + 1] = "image_arrow_top";
    });
    $("#arrow_bottom").click(function () {
        mouseDraw();
        diagramName[diagramID + 1] = "image_arrow_bottom";
    });

    //-------------------------------  도형 끝  --------------------------------------//

    //------------------------------- 정렬 부분 ------------------------------------//
    var tempimg = new Array();
    var pWidth, pHeight, pLeft, pTop, pRight, pBottom;
    var preimg, curimg, setimg, stdID, gap = 5;

    // 이미지 정보 구조체
    function imgInfo(id, width, height, left, top) {
        this.id = id;
        this.width = width;
        this.height = height;
        this.left = left;
        this.top = top;
        this.right = left + width;
        this.bottom = top + height;
        this.on = 0;
    }

    // 이미지 크기, 위치 설정
    function setImage(id, w, h, l, t) {
        $('#d' + id).removeClass("ui-resizable-autohide");
        $('#' + id).resizable('destroy');
        $('#' + id).css('width', w + 'px');
        $('#' + id).css('height', h + 'px');
        $('#' + id).offset({ top: t, left: l });
        if (ratioOn) $('#' + id).resizable({ handles: "n,s,e,w,ne,nw,se,sw", stop: function () { recodeUndo(); }, aspectRatio: true });
        else $('#' + id).resizable({ handles: "n,s,e,w,ne,nw,se,sw", stop: function () { recodeUndo(); } });
    }

    //기준 이미지 설정
    function setStd() {
        var min = 10000;
        for (var i = 1; i < curimg + 1; i++) {
            if (tempimg[i].on == 0) {
                if (Math.sqrt(Math.pow(tempimg[i].left, 2) + Math.pow(tempimg[i].top - 50, 2)) < min) {
                    min = Math.sqrt(Math.pow(tempimg[i].left, 2) + Math.pow(tempimg[i].top - 50, 2));
                    stdID = i;
                }
            }
        }
        if (curimg != 0) {
            pWidth = tempimg[stdID].width;
            pHeight = tempimg[stdID].height;
            pLeft = tempimg[stdID].left;
            pTop = tempimg[stdID].top;
            pRight = tempimg[stdID].right;
            pBottom = tempimg[stdID].bottom;
            setImage(stdID, pWidth, pHeight, pLeft, pTop);
        }
    }

    // 패널 아래 이미지 탐색
    function bottomInfo() {
        var list = new Array();
        var j = 1;
        for (var i = 1; i < curimg + 1; i++) {
            if (pBottom < tempimg[i].top && tempimg[i].top < pBottom + tempimg[i].height / 2) {
                if (tempimg[i].left < pRight) {
                    list[j] = i;
                    j++;
                }
            }
        }
        return list;
    }

    // 패널 오른쪽 이미지 탐색
    function rightInfo() {
        var list = new Array();
        var j = 1;
        for (var i = 1; i < curimg + 1; i++) {
            if (pRight < tempimg[i].left && tempimg[i].left < pRight + tempimg[i].width / 2) {
                if (tempimg[i].top < pBottom) {
                    list[j] = i;
                    j++;
                }
            }
        }
        return list;
    }

    // 아래 이미지 매치
    function bottomMatch(top, left, width, height) {
        var l1, l2, k;
        for (var i = 1; i < curimg + 1; i++) {
            if (tempimg[i].on == 0) {
                if (tempimg[i].top > top + height && tempimg[i].top < top + height + tempimg[i].height / 2) {
                    if (left > tempimg[i].left) l1 = left;
                    else l1 = tempimg[i].left;
                    if (left + width < tempimg[i].right) l2 = left + width;
                    else l2 = tempimg[i].right;
                    if (width > tempimg[i].width) k = (l2 - l1) / width;
                    else k = (l2 - l1) / tempimg[i].width;
                    if (k > 0.65) return i;
                }
            }
        }
        return 0;
    }

    // 오른쪽 이미지 매치
    function rightMatch(top, left, width, height) {
        var l1, l2, k;
        for (var i = 1; i < curimg + 1; i++) {
            if (tempimg[i].on == 0) {
                if (tempimg[i].left > left + width && tempimg[i].left < left + width + tempimg[i].width / 2) {
                    if (top > tempimg[i].top) l1 = top;
                    else l1 = tempimg[i].top;
                    if (top + height < tempimg[i].bottom) l2 = top + height;
                    else l2 = tempimg[i].bottom;
                    if (height > tempimg[i].height) k = (l2 - l1) / height;
                    else k = (l2 - l1) / tempimg[i].height;
                    if (k > 0.65) return i;
                }
            }
        }
        return 0;
    }

    // 아래 이미지 정렬
    function bottomSet(list) {
        var tempLeft, tempWidth, tempTop, tempHeight = 0;
        var n, min, t, add, subTop, subHeight;

        tempTop = pBottom + gap;
        tempLeft = pLeft;
        tempWidth = pWidth;
        n = list.length - 1;
        setimg -= n;                // 정렬할 이미지 갯 수 감소

        // 아래 이미지 수만큼 실행
        while (n > 1) {
            min = pRight;
            for (var i = 1; i < list.length ; i++) {
                if (min > tempimg[list[i]].left && tempimg[list[i]].on == 0) {
                    min = tempimg[list[i]].left;
                    t = list[i];
                }
            }
            // 처음 시작 시
            if (tempHeight == 0) {
                tempHeight = tempimg[t].height;
                pHeight += tempHeight + gap;
                pBottom += tempHeight + gap;
                setImage(tempimg[t].id, tempimg[t].width, tempHeight, tempLeft, tempTop);
                // 추가 이미지 탐색 및 정렬
                add = bottomMatch(tempTop, tempLeft, tempimg[t].width, tempHeight);
                subTop = tempTop + tempHeight + gap;
                while (add) {
                    tempHeight += tempimg[add].height + gap;
                    pHeight += tempimg[add].height + gap;
                    pBottom += tempimg[add].height + gap;
                    setImage(tempimg[add].id, tempimg[t].width, tempimg[add].height, tempLeft, subTop);
                    subTop += tempimg[add].height + gap;
                    tempimg[add].on = 1;
                    setimg--;
                    add = bottomMatch(subTop - tempimg[add].height - gap, tempLeft, tempimg[t].width, tempimg[add].height);
                }
            }
                // 중간 이미지 처리
            else {
                add = bottomMatch(tempTop, tempLeft, tempimg[t].width, tempimg[t].height);
                if (add) setImage(tempimg[t].id, tempimg[t].width, tempimg[t].height, tempLeft, tempTop);
                else setImage(tempimg[t].id, tempimg[t].width, tempHeight, tempLeft, tempTop);
                subTop = tempTop + tempimg[t].height + gap;
                subHeight = tempHeight - tempimg[t].height - gap;
                while (add) {
                    if (subHeight > 0) {
                        if (bottomMatch(subTop, tempLeft, tempimg[t].width, tempimg[add].height)) {
                            setImage(tempimg[add].id, tempimg[t].width, tempimg[add].height, tempLeft, subTop);
                            subTop += tempimg[add].height + gap;
                            subHeight -= (tempimg[add].height + gap);
                            tempimg[add].on = 1;
                            setimg--;
                            add = bottomMatch(subTop - tempimg[add].height - gap, tempLeft, tempimg[t].width, tempimg[add].height);
                        }
                        else {
                            setImage(tempimg[add].id, tempimg[t].width, subHeight, tempLeft, subTop);
                            tempimg[add].on = 1;
                            setimg--;
                            add = 0;
                        }
                    }
                    else {
                        add = 0;
                    }
                }
            }
            tempimg[t].on = 1;
            tempLeft += (tempimg[t].width + gap);
            tempWidth -= (tempimg[t].width + gap);
            if (tempWidth < 0) return -1;
            n--;
        }
        for (var i = 1; i < list.length ; i++) {
            if (tempimg[list[i]].on == 0) t = list[i];
        }
        if (tempHeight == 0) {
            tempHeight = tempimg[t].height;
            pHeight += tempHeight + gap;
            pBottom += tempHeight + gap;
        }

        // 마지막 이미지 처리
        if (tempHeight > tempimg[t].height + gap) {
            add = bottomMatch(tempTop, tempLeft, tempWidth, tempimg[t].height);
            if (add) setImage(tempimg[t].id, tempWidth, tempimg[t].height, tempLeft, tempTop);
            else setImage(tempimg[t].id, tempWidth, tempHeight, tempLeft, tempTop);

            subTop = tempTop + tempimg[t].height + gap;
            subHeight = tempHeight - tempimg[t].height - gap;

            while (add) {
                if (subHeight > 0) {
                    if (bottomMatch(subTop, tempLeft, tempWidth, tempimg[add].height)) {
                        setImage(tempimg[add].id, tempWidth, tempimg[add].height, tempLeft, subTop);
                        subTop += tempimg[add].height + gap;
                        subHeight -= (tempimg[add].height + gap);
                        tempimg[add].on = 1;
                        setimg--;
                        add = bottomMatch(subTop - tempimg[add].height - gap, tempLeft, tempWidth, tempimg[add].height);
                    }
                    else {
                        setImage(tempimg[add].id, tempWidth, subHeight, tempLeft, subTop);
                        tempimg[add].on = 1;
                        setimg--;
                        add = 0;
                    }
                }
                else {
                    add = 0;
                }
            }
        }
        else {
            setImage(tempimg[t].id, tempWidth, tempHeight, tempLeft, tempTop);
        }
        tempimg[t].on = 1;
    }

    // 오른쪽 이미지 정렬
    function rightSet(list) {
        var tempLeft, tempHeight, tempTop, tempWidth = 0;
        var n, min, t, add, subLeft, subWidth;

        tempTop = pTop;
        tempLeft = pRight + gap;
        tempHeight = pHeight;
        n = list.length - 1;
        setimg -= n;

        while (n > 1) {
            min = pBottom;
            for (var i = 1; i < list.length ; i++) {
                if (min > tempimg[list[i]].top && tempimg[list[i]].on == 0) {
                    min = tempimg[list[i]].top;
                    t = list[i];
                }
            }
            // 처음 시작 시
            if (tempWidth == 0) {
                tempWidth = tempimg[t].width;
                pWidth += tempWidth + gap;
                pRight += tempWidth + gap;
                setImage(tempimg[t].id, tempWidth, tempimg[t].height, tempLeft, tempTop);
                add = rightMatch(tempTop, tempLeft, tempWidth, tempimg[t].height);
                subLeft = tempLeft + tempWidth + gap;
                while (add) {
                    tempWidth += tempimg[add].width + gap;
                    pWidth += tempimg[add].width + gap;
                    pRight += tempimg[add].width + gap;
                    setImage(tempimg[add].id, tempimg[add].width, tempimg[t].height, subLeft, tempTop);
                    subLeft += tempimg[add].width + gap;
                    tempimg[add].on = 1;
                    setimg--;
                    add = rightMatch(tempTop, subLeft - tempimg[add].width - gap, tempimg[add].width, tempimg[t].height);
                }
            }
                // 중간 이미지 처리
            else {
                add = rightMatch(tempTop, tempLeft, tempimg[t].width, tempimg[t].height);
                if (add) setImage(tempimg[t].id, tempimg[t].width, tempimg[t].height, tempLeft, tempTop);
                else setImage(tempimg[t].id, tempWidth, tempimg[t].height, tempLeft, tempTop);
                subLeft = tempLeft + tempimg[t].width + gap;
                subWidth = tempWidth - tempimg[t].width - gap;
                while (add) {
                    if (subWidth > 0) {
                        if (rightMatch(tempTop, subLeft, tempimg[add].width, tempimg[t].height)) {
                            setImage(tempimg[add].id, tempimg[add].width, tempimg[t].height, subLeft, tempTop);
                            tempTop += tempimg[add].width + gap;
                            subWidth -= (tempimg[add].width + gap);
                            tempimg[add].on = 1;
                            setimg--;
                            add = rightMatch(tempTop, subLeft - tempimg[add].width - gap, tempimg[add].width, tempimg[t].height);
                        }
                        else {
                            setImage(tempimg[add].id, subWidth, tempimg[t].height, subLeft, tempTop);
                            tempimg[add].on = 1;
                            setimg--;
                            add = 0;
                        }
                    }
                    else {
                        add = 0;
                    }
                }
            }
            tempimg[t].on = 1;
            tempTop += (tempimg[t].height + gap);
            tempHeight -= (tempimg[t].height + gap);
            if (tempHeight < 0) return -1;
            n--;
        }
        for (var i = 1; i < list.length ; i++) {
            if (tempimg[list[i]].on == 0) t = list[i];
        }
        if (tempWidth == 0) {
            tempWidth = tempimg[t].width;
            pWidth += tempWidth + gap;
            pRight += tempWidth + gap;
        }
        // 마지막 이미지 처리
        if (tempWidth > tempimg[t].width + gap) {
            add = rightMatch(tempTop, tempLeft, tempimg[t].width, tempHeight);
            if (add) setImage(tempimg[t].id, tempimg[t].width, tempHeight, tempLeft, tempTop);
            else setImage(tempimg[t].id, tempWidth, tempHeight, tempLeft, tempTop);

            subLeft = tempLeft + tempimg[t].width + gap;
            subWidth = tempWidth - tempimg[t].width - gap;

            while (add) {
                if (subWidth > 0) {
                    if (rightMatch(tempTop, subLeft, tempimg[add].width, tempHeight)) {
                        setImage(tempimg[add].id, tempimg[add].width, tempHeight, subLeft, tempTop);
                        subLeft += tempimg[add].width + gap;
                        subHeight -= (tempimg[add].width + gap);
                        tempimg[add].on = 1;
                        setimg--;
                        add = rightMatch(tempTop, subLeft - tempimg[add].width - gap, tempimg[add].width, tempHeight);
                    }
                    else {
                        setImage(tempimg[add].id, subWidth, tempHeight, subLeft, tempTop);
                        tempimg[add].on = 1;
                        setimg--;
                        add = 0;
                    }
                }
                else {
                    add = 0;
                }
            }
        }
        else {
            setImage(tempimg[t].id, tempWidth, tempHeight, tempLeft, tempTop);
        }
        tempimg[t].on = 1;
    }

    // 단계별 이미지 정렬
    function setLayout() {
        var result, rMat, bMat, rHeight = 0, bWidth = 0;
        var rList = new Array();
        var bList = new Array();

        preimg = setimg;
        rList = rightInfo();
        bList = bottomInfo();
        for (var i = 1; i < rList.length ; i++) {
            rHeight += tempimg[rList[i]].height;
        }
        for (var i = 1; i < bList.length; i++) {
            bWidth += tempimg[bList[i]].width;
        }
        rMat = Math.abs(1 - rHeight / pHeight);
        bMat = Math.abs(1 - bWidth / pWidth);

        if (rMat < bMat) {
            result = rightSet(rList);
        }
        else if (bWidth > 0) {
            result = bottomSet(bList);
        }
        if (result == -1) {
            $.gritter.add({
                title: "모든 이미지 정렬에 실패 하였습니다.",
                text: "이미지를 겹치지 마세요"
            });
        }
        else {
            if (preimg == 1) { }
            else if (preimg == setimg) {
                $.gritter.add({
                    title: "모든 이미지 정렬에 실패 하였습니다.",
                    text: "이미지를 겹치지 마세요"
                });
                return;
            }
            else if (setimg > 0) {
                setLayout();
            }
        }
    }

    // 정렬 버튼
    $('#layout').click(function () {
        var a = new Array();
        var j = 1;
        // 현재 이미지 정보 저장
        for (var i = 1; i < imageID; i++) {
            if (alive[i]) {
                var offset = $('#' + i).offset();
                tempimg[j] = new imgInfo(i, image[i].width, image[i].height, offset.left, offset.top);
                j++;
            }
        }
        curimg = j - 1;
        setimg = curimg;
        setStd();
        setLayout();
        hidehandle();
        recodeUndo();
    });
    //--------------------------------------- 정렬 끝 -------------------------------------------------//

    //--------------------------------------텍스트 부분------------------------------------------------//
    var textID = 1;
    var maxTextNum = 5;
    var focusText = 0;
    var textOn = 1;
    var fontSizeArr = ["8", "8", "10", "12", "14", "18", "24", "36"];
    var aliveT = new Array();

    // 크롬 폰트 초기화
    function initfont() {
        if ($('#text-area' + focusText).text() == '') {
            for (i = 0; i < 3; i++) {
                document.execCommand('justifycenter', false, null);
                document.execCommand('justifyleft', false, null);
                document.execCommand("FontSize", false, "7");
                document.execCommand("FontName", false, "돋움체");
                document.execCommand("forecolor", false, "white");
            }
        }
    }

    // 텍스트 컨트롤 생성
    function setTextControl() {
        textOn = 1;
        $('#t' + focusText).resizable('enable');
        $('#dt' + focusText).draggable('enable');
        $('#toolbar' + focusText).show();
        var temp_top = parseInt($('#dt' + focusText).css('top'));
        temp_top -= parseInt($('#toolbar' + focusText).css('height'));
        $('#dt' + focusText).css('top', parseInt(temp_top));
        if (!agent.isMSIE) initfont();
    }

    // 텍스트 컨트롤 제거
    function deleteTextControl(focus) {
        textOn = 0;
        $('#t' + focusText).resizable('disable');
        $('#dt' + focusText).draggable('disable');
        $('#toolbar' + focusText).hide();
        var temp_top = parseInt($('#dt' + focusText).css('top'));
        temp_top += parseInt($('#toolbar' + focusText).css('height'));
        $('#dt' + focusText).css('top', parseInt(temp_top));
        if (focus != 1) focusText = 0;
    }

    // text버튼 클릭
    $('#text').click(function () {
        var contents;
        if (textID > maxTextNum) {
            $.gritter.add({
                title: "text객체는 5개 이상 생성할 수 없습니다."
            });
            return;
        }
        if (agent.isMSIE) contents = '<p align="left"><font color="#ffffff" face="돋움체" size="7">텍스트를 입력하세요</font></p>';
        else contents = '<div style="text-align: left;"><font color="#ffffff" face="돋움체" size="7">텍스트를 입력하세요</font></div>';
        textView(textID, contents, 500, 300 + (textID % 4) * 50, 300 + (textID % 4) * 50);
        recodeUndo();
        textID++;
    });

    // 텍스트 생성
    function textView(id, contents, width, left, top) {
        $('#editor').append('<div id="dt' + id + '"><div id="t' + id + '" style="width:' + width + 'px;"><div id="toolbar' + id + '"></div><div id="text-area' + id + '" class="text-area" style="border:#d3d3d3 solid 1px;"></div></div></div>');
        $('#text-area' + id).wysiwyg({ toolbar_selector: "#toolbar" + id });
        $('#text-area' + id).wysiwyg("edit", true);
        $('#text-area' + id).append(contents);

        //$('#toolbar' + id).css('border-top', '10px solid #9CBCD9');
        $('#toolbar' + id).css('border', '5px solid #CCC6BC');
        $('#toolbar' + id).css('background-color', '#CCC6BC');

        // 드래그 허용
        $('#dt' + id).css('z-index', '1000');
        $('#dt' + id).css('display', 'inline-block');
        $('#dt' + id).css('position', 'absolute');
        $('#dt' + id).offset({ top: top, left: left });
        $('#dt' + id).draggable({
            cursor: "move",
            stop: function () { recodeUndo(); },
            handle: "#toolbar" + id,
            cancel: "#text-area" + id
        });

        // 리사이즈 허용
        $('#t' + id).resizable({ handles: "w, e", stop: function () { recodeUndo(); }, minWidth: 450 });

        // 텍스트 마우스 다운 이벤트
        $('#dt' + id).mousedown(function () {
            if (dragFlag) return;
            clickOn = 1;
            focusID = 0;
            focusDiagram = 0;
            hidehandle();
            if (textOn) deleteTextControl(0);
            var id = this.id.split('t');
            focusText = id[1];
            if (textOn == 0) {
                setTextControl();
                textOn = 1;
            }
        });

        // 텍스트 블록지정을 위해 추가  
        $('#text-area' + id).mousedown(function () {
            if (dragFlag) return;
            $(document).unbind("mousemove");
        });

        // x축 오버플로우 hide 및 텍스트 쪼개기
        $('.text-area').css("overflow-x", "hidden");
        $('.text-area').css("white-space", "nowrap");          // 자동 줄바꿈 금지

        if (textOn) deleteTextControl(0);
        hidehandle();
        setTextControl();
        aliveT[id] = 1;
        focusText = id;
        focusID = 0;
        focusDiagram = 0;
    }

    // 현재 폰트 태그 정보
    function fontInfoCur(color, face, size, style) {
        this.text = "";
        this.color = color;
        this.face = face;
        this.size = size;
        this.style = style;
        this.strong = 0;
        this.em = 0;
        this.u = 0;
        this.strike = 0;
    }

    // 폰트 정보 배열
    function fontInfoArr() {
        this.colorArr = new Array();
        this.colorIdx = 0;
        this.faceArr = new Array();
        this.faceIdx = 0;
        this.sizeArr = new Array();
        this.sizeIdx = 0;
        this.styleArr = new Array();
        this.styleArr = 0;
        this.caseArr = new Array();
        this.caseIdx = 0;
    }

    // 텍스트 박스 프린트
    function printTextBox(ctx, id) {
        var align, tempData, tagName;
        var ix, bw, bh, bTop, fontWidth, penalWidth, penalTop = 0;
        if (agent.isMSIE) tagName = "p";
        else tagName = "div";
        penalWidth = $('#t' + id + ' #text-area' + id + ' ' + tagName + ':nth-child(1)').width();
        penalTop = parseInt($('#t' + id + ' #text-area' + id + ' ' + tagName + ':nth-child(1)').css('height'));
        penalTop -= parseInt($('#t' + id + ' #text-area' + id + ' ' + tagName + ':nth-child(1) font').css('height'));
        penalTop /= 2;
        for (var i = 1; i <= $('#t' + id + ' #text-area' + id + ' ' + tagName).length; i++) {
            if ($('#t' + id + ' #text-area' + id + ' ' + tagName + ':nth-child(' + i + ')').html()) {
                if (agent.isMSIE) align = $('#t' + id + ' #text-area' + id + ' ' + tagName + ':nth-child(' + i + ')').attr('align');
                else align = $('#t' + id + ' #text-area' + id + ' ' + tagName + ':nth-child(' + i + ')').css('text-align');
                fontWidth = $('#t' + id + ' #text-area' + id + ' ' + tagName + ':nth-child(' + i + ') font').width();
                tempData = searchPanel(i, id, tagName);
                ix = 0;

                if (align == 'center' && fontWidth < penalWidth) ix = parseInt((penalWidth - fontWidth) / 2);
                if (align == 'right' && fontWidth < penalWidth) ix = parseInt(penalWidth - fontWidth);

                bTop = parseInt($('#t' + id + ' #text-area' + id + ' ' + tagName + ':nth-child(' + i + ')').css('height'));
                bTop -= parseInt($('#t' + id + ' #text-area' + id + ' ' + tagName + ':nth-child(' + i + ')').css('margin-bottom'));
                bTop -= parseInt($('#t' + id + ' #text-area' + id + ' ' + tagName + ':nth-child(' + i + ') font').css('height'));
                bh = parseInt($('#t' + id + ' #text-area' + id + ' ' + tagName + ':nth-child(' + i + ') font').css('height'));
                penalTop += parseInt($('#t' + id + ' #text-area' + id + ' ' + tagName + ':nth-child(' + i + ') font').css('height'));

                for (j = 0; j < tempData.length; j++) {
                    if (tempData[j].style) {
                        bw = getFontSizeWidth(tempData[j]);
                        //bh = getFontSizeHeight(tempData[j]);  //텍스트 크기별 색 채우기
                        if (agent.isMSIE) drawBackground(ix, penalTop - bh, bw, bh, tempData[j].style, ctx);
                        else drawBackground(ix, penalTop - bh, bw, bh, tempData[j].style, ctx);
                    }
                    drawText(ix, penalTop, tempData[j], ctx);
                    ix += getFontSizeWidth(tempData[j]);
                }

                penalTop -= parseInt($('#t' + id + ' #text-area' + id + ' ' + tagName + ':nth-child(' + i + ') font').css('height'));
                penalTop += parseInt($('#t' + id + ' #text-area' + id + ' ' + tagName + ':nth-child(' + i + ')').css('height'));
                penalTop += parseInt($('#t' + id + ' #text-area' + id + ' ' + tagName + ':nth-child(' + i + ')').css('margin-bottom'));
            }
        }
    }

    // 폰트 size
    function getFontSizeWidth(data) {
        var temp = data.text.replace(' ', '&nbsp;');
        if (data.strong) temp = '<strong>' + temp + '</strong>';
        if (data.em) temp = '<em>' + temp + '</em>';
        if (data.u) temp = '<u>' + temp + '</u>';
        if (data.strike) temp = '<strike>' + temp + '</strike>';
        $('#font-testing').find('font').html(temp);
        $('#font-testing').find('font').attr('face', data.face);
        $('#font-testing').find('font').attr('size', data.size);
        return $('#font-testing').find('font').width();
    }
    function getFontSizeHeight(data) {
        var temp = data.text.replace(' ', '&nbsp;');
        if (data.strong) temp = '<strong>' + temp + '</strong>';
        if (data.em) temp = '<em>' + temp + '</em>';
        if (data.u) temp = '<u>' + temp + '</u>';
        if (data.strike) temp = '<strike>' + temp + '</strike>';
        $('#font-testing').find('font').html(temp);
        $('#font-testing').find('font').attr('face', data.face);
        $('#font-testing').find('font').attr('size', data.size);
        return $('#font-testing').find('font').height();
    }

    // 텍스트 패널 탐색 (n번째, top 여백)
    function searchPanel(n, id, tag) {
        var parent = $('#t' + id + ' #text-area' + id + ' ' + tag + ':nth-child(' + n + ')');
        var panelValue = parent.html();
        var panelAlign = parent.attr('align');
        var panelWidth = parseInt(parent.css('width'));
        var panelHeight = parseInt(parent.css('height'));
        var textArr = new Array();
        var textNum = 0;
        var infoCur = new fontInfoCur("#ffffff", "돋움", "7", "");
        var infoArr = new fontInfoArr();
        var tagName = "", closeTag = 0;

        // 패널 전체 탐색
        for (var i = 0; i < panelValue.length; i++) {
            //태그 탐색
            if (panelValue[i] == '<') {
                i++;
                if (panelValue[i] == '/') {
                    closeTag = 1;
                    i++;
                }
                tagName = "";
                while (panelValue[i] != '>' && panelValue[i] != ' ' && i < panelValue.length) {
                    tagName += panelValue[i];
                    i++;
                }
            }
            // 폰트 태그 탐색
            if (tagName == "font" || tagName == "span") {
                if (closeTag) {
                    if (infoArr.caseArr[infoArr.caseIdx] % 2 == 0) {
                        infoArr.colorIdx--;
                        infoCur.color = infoArr.colorArr[infoArr.colorIdx];
                    }
                    if (infoArr.caseArr[infoArr.caseIdx] % 3 == 0) {
                        infoArr.faceIdx--;
                        infoCur.face = infoArr.faceArr[infoArr.faceIdx];
                    }
                    if (infoArr.caseArr[infoArr.caseIdx] % 5 == 0) {
                        infoArr.sizeIdx--;
                        infoCur.size = infoArr.sizeArr[infoArr.sizeIdx];
                    }
                    if (infoArr.caseArr[infoArr.caseIdx] % 7 == 0) {
                        infoArr.styleIdx--;
                        infoCur.style = infoArr.styleArr[infoArr.styleIdx];
                    }
                    infoArr.caseIdx--;
                    closeTag = 0;
                }
                else {
                    infoArr.caseIdx++;
                    infoArr.caseArr[infoArr.caseIdx] = 1;
                    if (panelValue[i] == ' ') i++;
                    while (panelValue[i] != '>' && i < panelValue.length) {
                        var tempInfo = "";
                        while (panelValue[i] != '=' && i < panelValue.length) {
                            tempInfo += panelValue[i];
                            i++;
                        }
                        i += 2;
                        var tempData = "";
                        while (panelValue[i] != '"' && i < panelValue.length) {
                            tempData += panelValue[i];
                            i++;
                        }
                        i++;
                        if (tempInfo == "color") {
                            infoArr.caseArr[infoArr.caseIdx] *= 2;
                            infoArr.colorIdx++;
                            infoCur.color = infoArr.colorArr[infoArr.colorIdx] = tempData;
                        }
                        else if (tempInfo == "face") {
                            infoArr.caseArr[infoArr.caseIdx] *= 3;
                            infoArr.faceIdx++;
                            infoCur.face = infoArr.faceArr[infoArr.faceIdx] = tempData;
                        }
                        else if (tempInfo == "size") {
                            infoArr.caseArr[infoArr.caseIdx] *= 5;
                            infoArr.sizeIdx++;
                            if (tempData.length > 4) tempData = 7;
                            infoCur.size = infoArr.sizeArr[infoArr.sizeIdx] = tempData;
                        }
                        else if (tempInfo == "style") {
                            infoArr.caseArr[infoArr.caseIdx] *= 7;
                            infoArr.styleIdx++;
                            infoCur.style = infoArr.styleArr[infoArr.styleIdx] = tempData;
                        }
                        if (panelValue[i] == ' ') i++;
                    }
                }
                tagName = "";
            }
            else if (tagName == "strong" || tagName == "b") {
                if (closeTag) {
                    infoCur.strong = 0;
                    closeTag = 0;
                }
                else {
                    infoCur.strong = 1;
                }
                tagName = "";
            }
            else if (tagName == "em" || tagName == "i") {
                if (closeTag) {
                    infoCur.em = 0;
                    closeTag = 0;
                }
                else {
                    infoCur.em = 1;
                }
                tagName = "";
            }
            else if (tagName == "u") {
                if (closeTag) {
                    infoCur.u = 0;
                    closeTag = 0;
                }
                else {
                    infoCur.u = 1;
                }
                tagName = "";
            }
            else if (tagName == "strike") {
                if (closeTag) {
                    infoCur.strike = 0;
                    closeTag = 0;
                }
                else {
                    infoCur.strike = 1;
                }
                tagName = "";
            }
            else if (tagName == "br") {
                if (closeTag) closeTag = 0;
                textArr[textNum] = new fontInfoCur("#ffffff", "돋움", "7", "");
                textArr[textNum].color = infoCur.color;
                textArr[textNum].face = infoCur.face;
                textArr[textNum].size = infoCur.size;
                textNum++;
                tagName = "";
            }
            else {
                var tempText = "";
                if (panelValue[i] == '>') i++;
                if (panelValue[i] != '<') tempText = panelValue[i];
                while (panelValue[i + 1] != '<' && i < panelValue.length) {
                    i++;
                    tempText += panelValue[i];
                }
                if (tempText.length) {
                    textArr[textNum] = new fontInfoCur("#ffffff", "돋움", "7", "");
                    textArr[textNum].text = tempText;
                    textArr[textNum].color = infoCur.color;
                    textArr[textNum].face = infoCur.face;
                    textArr[textNum].size = infoCur.size;
                    textArr[textNum].style = infoCur.style;
                    textArr[textNum].strong = infoCur.strong;
                    textArr[textNum].em = infoCur.em;
                    textArr[textNum].u = infoCur.u;
                    textArr[textNum].strike = infoCur.strike;
                    textNum++;
                }
            }
        }
        /*
        for (var i = 0; i < textArr.length; i++) {
            alert(textArr[i].text + '\n' + textArr[i].color + '\n' + textArr[i].face + '\n' + textArr[i].size + '\n' + textArr[i].style + '\n' + textArr[i].strong + '\n' + textArr[i].em + '\n' + textArr[i].u + '\n' + textArr[i].strike);
        }*/

        return textArr;
    }

    // 배경 그리기
    function drawBackground(x, y, w, h, style, ctx) {
        var idx = style.indexOf('background-color: ');
        var color = "";
        if (idx >= 0) {
            idx += 18;
            while (style[idx] != ';') {
                color += style[idx];
                idx++;
            }
            ctx.fillStyle = color;
            ctx.fillRect(x, y, w, h);
        }
    }

    // 글자 그리기
    function drawText(x, y, info, ctx) {
        if (info.face) {
            var tempText = info.text.replace(/&nbsp;/g, ' ');
            ctx.fillStyle = info.color;
            ctx.font = fontSizeArr[info.size] + 'pt ' + info.face;
            ctx.textBaseline = 'bottom';
            if (info.strong) ctx.font = 'bold ' + ctx.font;
            if (info.em) ctx.font = 'italic ' + ctx.font;
            ctx.fillText(tempText, x, y);
        }
    }
    //--------------------------------------텍스트 끝----------------------------------------------//

    //---------------------------------- redo, undo 부분 ------------------------------------------//
    var recodeIdx = 0;
    var maxRecodeIdx = 0;
    var minRecodeIdx = 0;
    var recodeArrLength = 50;
    var recodeArr = new Array();

    recodeArr[0] = new recodeStruct();

    // 기본 정보
    function infoList() {
        this.width;
        this.height;
        this.top;
        this.left;
        this.contents;
    }

    // 객체 정보 구조체
    function objectInfo() {
        this.imageInfo = new Array();
        this.diagramInfo = new Array();
        this.textInfo = new Array();

        for (var i = 1; i < imageID; i++) {
            if (alive[i]) {
                var offset = $('#' + i).offset();
                this.imageInfo[i] = new infoList();
                this.imageInfo[i].width = parseInt($('#' + i).css('width'));
                this.imageInfo[i].height = parseInt($('#' + i).css('height'));
                this.imageInfo[i].top = offset.top;
                this.imageInfo[i].left = offset.left;
            }
        }
        for (var i = 1; i <= diagramID; i++) {
            if (aliveD[i]) {
                var offset = $('#a' + i).offset();
                this.diagramInfo[i] = new infoList();
                this.diagramInfo[i].width = parseInt($('#a' + i).css('width'));
                this.diagramInfo[i].height = parseInt($('#a' + i).css('height'));
                this.diagramInfo[i].top = offset.top;
                this.diagramInfo[i].left = offset.left;
            }
        }
        for (var i = 1; i < textID; i++) {
            if (aliveT[i]) {
                var offset = $('#t' + i).offset();
                this.textInfo[i] = new infoList();
                this.textInfo[i].width = parseInt($('#t' + i).css('width'));
                this.textInfo[i].top = offset.top;
                this.textInfo[i].left = offset.left;
                this.textInfo[i].contents = $('#text-area' + i).html();
            }
        }
    }

    // 데이터 기록 구조체
    function recodeStruct() {
        this.source = $('#editor').html();
        this.imageID = imageID;
        this.focusID = focusID;
        this.image = new Array();
        this.imagePath = new Array();
        this.alive = new Array();
        this.zidx = new Array();
        this.diagramID = diagramID;
        this.focusDiagram = focusDiagram;
        this.diagramName = new Array();
        this.aliveD = new Array();
        this.textID = textID;
        this.focusText = focusText;
        this.textOn = textOn;
        this.aliveT = new Array();
        this.objInfo = new objectInfo();

        transArr(this.image, image);
        transArr(this.imagePath, imagePath);
        transArr(this.alive, alive);
        transArr(this.zidx, zidx);
        transArr(this.diagramName, diagramName);
        transArr(this.aliveD, aliveD);
        transArr(this.aliveT, aliveT);
    }

    // 배열 값 전달
    function transArr(buffer, arr) {
        for (i = 0; i < arr.length; i++) {
            buffer[i] = arr[i];
        }
    }

    // 저장한 데이터 셋
    function setRedoData(data) {
        $('#editor').html("");
        imageID = data.imageID;
        focusID = data.focusID;
        image = data.image;
        imagePath = data.imagePath;
        alive = data.alive;
        zidx = data.zidx;
        diagramID = data.diagramID;
        focusDiagram = data.focusDiagram;
        diagramName = data.diagramName;
        aliveD = data.aliveD;
        textID = data.textID;
        focusText = data.focusText;
        textOn = data.textOn;
        aliveT = data.aliveT;
        setDataEvent(data);
    }

    // 객체 이벤트 설정
    function setDataEvent(data) {
        for (var i = 1; i < imageID; i++) {
            if (alive[i]) {
                imageView(i, imagePath[i], data.objInfo.imageInfo[i].width, data.objInfo.imageInfo[i].height, data.objInfo.imageInfo[i].left, data.objInfo.imageInfo[i].top, 0);
            }
        }
        for (var i = 1; i <= diagramID; i++) {
            if (aliveD[i]) {
                diagramView(i, data.diagramName[i], data.objInfo.diagramInfo[i].width, data.objInfo.diagramInfo[i].height, data.objInfo.diagramInfo[i].left, data.objInfo.diagramInfo[i].top);
            }
        }
        for (var i = 1; i < textID; i++) {
            if (aliveT[i]) {
                textView(i, data.objInfo.textInfo[i].contents, data.objInfo.textInfo[i].width, data.objInfo.textInfo[i].left, data.objInfo.textInfo[i].top);
            }
        }
    }

    // 데이터 기록
    function recodeUndo() {
        recodeIdx++;
        if (recodeIdx % recodeArrLength == minRecodeIdx % recodeArrLength) minRecodeIdx++;
        maxRecodeIdx = recodeIdx;
        recodeArr[recodeIdx % recodeArrLength] = new recodeStruct();
        if (minRecodeIdx > 2 * recodeArrLength) {
            minRecodeIdx -= recodeArrLength;
            recodeIdx -= recodeArrLength;
        }
    }

    // 이전 버튼
    $('#undo').click(function () {
        if (recodeIdx > minRecodeIdx) {
            recodeIdx--;
            setRedoData(recodeArr[recodeIdx % recodeArrLength]);
        }
    });

    // 다음 버튼
    $('#redo').click(function () {
        if (recodeIdx < maxRecodeIdx) {
            recodeIdx++;
            setRedoData(recodeArr[recodeIdx % recodeArrLength]);
        }
    });

    //----------------------------------- redo, undo 끝 -------------------------------------------//


    //------------------------------------ setting 부분 -------------------------------------------//
    var c_gap = 0;
    var backColor = "";
    set_init();

    function set_init() {
        for (var i = 0; i <= 7; i++) {
            $('#bk' + i).click(function () {
                var id = this.id[2];
                if (id == 0) backColor = "";
                else if (id == 1) backColor = "#ffffff";
                else if (id == 2) backColor = "#fffff0";
                else if (id == 3) backColor = "#e6e6fa";
                else if (id == 4) backColor = "#8EFFD4";
                else if (id == 5) backColor = "#fff0f5";
                else if (id == 6) backColor = "#f5f5dc";
                else if (id == 7) backColor = "#add8e6";
                drawPreviewCanvas();
            });
        }
        for (var i = 0; i <= 5; i++) {
            $('#gap' + i).click(function () {
                var id = this.id[3];
                if (id == 0) c_gap = 0;
                else if (id == 1) c_gap = 5;
                else if (id == 2) c_gap = 10;
                else if (id == 3) c_gap = 20;
                else if (id == 4) c_gap = 30;
                else if (id == 5) c_gap = 50;
                drawPreviewCanvas();
            });
        }
    }


    //------------------------------------- setting 끝 --------------------------------------------//
}