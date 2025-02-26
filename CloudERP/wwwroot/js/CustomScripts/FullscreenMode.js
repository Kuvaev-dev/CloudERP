$(document).ready(function () {
    $('#fullscreenButton').on('click', function () {
        toggleFullScreen();
    });

    document.addEventListener('fullscreenchange', function () {
        updateIcon();
    });
});

function toggleFullScreen() {
    if (!document.fullscreenElement) {
        document.documentElement.requestFullscreen();
    } else {
        if (document.exitFullscreen) {
            document.exitFullscreen();
        }
    }
}

function updateIcon() {
    if (document.fullscreenElement) {
        $('#fullscreenButton').removeClass('bi-fullscreen').addClass('bi-fullscreen-exit');
    } else {
        $('#fullscreenButton').removeClass('bi-fullscreen-exit').addClass('bi-fullscreen');
    }
}