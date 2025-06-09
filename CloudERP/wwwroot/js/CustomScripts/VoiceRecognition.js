let activeElement = null;
let recognition = null;
let isListening = false;

document.addEventListener('DOMContentLoaded', (event) => {
    document.addEventListener('focusin', (event) => {
        activeElement = event.target;
    });

    function initializeRecognition(culture) {
        if (!('webkitSpeechRecognition' in window)) {
            alert('Ваш браузер не поддерживает голосовой ввод.');
            return;
        }

        recognition = new webkitSpeechRecognition();
        recognition.continuous = false;
        recognition.interimResults = false;
        recognition.lang = culture;

        recognition.onstart = function () {
            console.log('Голосовой ввод начался.');
            isListening = true;
            updateButtonState();
        };

        recognition.onresult = function (event) {
            if (activeElement) {
                const transcript = event.results[0][0].transcript;
                if (activeElement.tagName === 'TEXTAREA' || activeElement.type === 'text') {
                    activeElement.value = transcript;
                }
            }
        };

        recognition.onerror = function (event) {
            if (event.error === 'no-speech') {
                console.log('Ошибка: Никакая речь не была обнаружена.');
            } else if (event.error === 'network') {
                alert('Ошибка сети: Пожалуйста, проверьте ваше интернет-соединение и попробуйте снова.');
            } else {
                alert('Ошибка голосового ввода: ' + event.error);
            }
        };

        recognition.onend = function () {
            console.log('Голосовой ввод завершен.');
            isListening = false;
            updateButtonState();
        };
    }

    function updateButtonState() {
        const buttonIcon = document.getElementById('voiceRecognitionButton').querySelector('i');
        if (isListening) {
            buttonIcon.style.color = 'red';
        } else {
            buttonIcon.style.color = 'gray';
        }
    }

    function toggleVoiceRecognition() {
        if (isListening) {
            recognition.stop();
        } else {
            recognition.start();
        }
        isListening = !isListening;
        updateButtonState();
    }

    const voiceRecognitionButton = document.getElementById('voiceRecognitionButton');
    voiceRecognitionButton.addEventListener('click', toggleVoiceRecognition);

    initializeRecognition(window.Culture || 'uk-UA');
});