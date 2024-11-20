function ShowImagePreview(imageUploader, previewImage) {
    if (imageUploader.files && imageUploader.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $(previewImage).attr('src', e.target.result);
        }
        reader.readAsDataURL(imageUploader.files[0]);
    }
}


//$(document).ready(function () {
//    $('#save').click(function () {
//        $.validator.unobtrusive.parse(myForm);
//        if ($(myForm).valid()) {
//            var file = $('#imageBrowes').get(0).files;
//            var data = new FormData(myForm);
//            data.append('Photo',file[0]);


//            var ajaxConfig = {
//                type: "POST",
//                url: "/Product/AddProduct",
//                data: data,
//                success: function (response) {
//                    alert("Save successfully!");
//                },
//                error: function (xhr, status) {
//                    alert('Something wrong :///');
//                }
//            }

//            if ($(myForm).attr('enctype') == "multipart/form-data") {
//                ajaxConfig["contentType"] = false;
//                ajaxConfig["processData"] = false;
//            }

//            $.ajax(ajaxConfig);
//        }
//    });
//});

