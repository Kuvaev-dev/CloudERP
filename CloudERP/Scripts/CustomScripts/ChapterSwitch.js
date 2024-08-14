$(document).ready(function () {
    let currentChapter = 1;
    const totalChapters = $('.chapter').length;

    function showChapter(chapterNumber) {
        $('.chapter').removeClass('active');
        $(`#chapter${chapterNumber}`).addClass('active');

        $('#prev-btn').prop('disabled', chapterNumber === 1);
        $('#next-btn').prop('disabled', chapterNumber === totalChapters);
    }

    $('.nav-link').on('click', function (e) {
        e.preventDefault();
        currentChapter = $(this).data('chapter');
        showChapter(currentChapter);
    });

    $('#prev-btn').on('click', function () {
        if (currentChapter > 1) {
            currentChapter--;
            showChapter(currentChapter);
        }
    });

    $('#next-btn').on('click', function () {
        if (currentChapter < totalChapters) {
            currentChapter++;
            showChapter(currentChapter);
        }
    });

    showChapter(currentChapter);
});