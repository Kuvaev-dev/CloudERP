$(document).ready(function () {
    // Обработчик клика по кнопке
    $('.nav-link').click(function () {
        // Переключение темы
        toggleTheme();
    });
});

function toggleTheme() {
    // Получаем основной контейнер
    var container = $('body');

    // Переключаем класс dark-theme
    container.toggleClass('dark-theme');

    // Переключаем иконку между солнцем и луной (в зависимости от темы)
    var icon = $('.nav-link i');
    icon.toggleClass('fa-sun fa-moon');

    // Вызываем функцию изменения цвета текста и фона
    changeColors();
}

function changeColors() {
    // Получаем основной контейнер
    var container = $('body');

    // Переключаем цвет текста и фона в зависимости от темы
    if (container.hasClass('dark-theme')) {
        container.css({ 'color': 'white', 'background-color': 'black' });
    } else {
        container.css({ 'color': '', 'background-color': '' });
    }
}
