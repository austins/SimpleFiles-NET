jQuery(document).ready(function($) {
    $("time").timeago();

    $(".previewImage").click(function() {
        var parent = $(this).parent();
        var fileLink = parent.find("a.fileLink");

        $("#previewModal .modal-body").html("<div class=\"text-center\">"
            + "<img src=\"" + fileLink.attr("href") + "\" alt=\"" + fileLink.text() + "\""
            + " style=\"max-width: 100%; height: auto;\"></div>");
    });
});