const tour = new Shepherd.Tour({
    defaultStepOptions: {
        classes: 'shadow-md bg-purple-dark',
        scrollTo: true,
        cancelIcon: {
            enabled: true
        }
    }
});

tour.addStep({
    id: 'step1',
    text: 'This is the first step of the tour.',
    attachTo: {
        element: '#step1',
        on: 'bottom'
    },
    buttons: [
        {
            text: 'Next',
            action: tour.next,
            classes: 'btn btn-info'
        }
    ]
});

tour.addStep({
    id: 'step2',
    text: 'This is the second step of the tour.',
    attachTo: {
        element: '#step2',
        on: 'bottom'
    },
    buttons: [
        {
            text: 'Back',
            action: tour.back,
            classes: 'btn btn-info'
        },
        {
            text: 'Finish',
            action: tour.complete,
            classes: 'btn btn-info'
        }
    ]
});

document.querySelector('#start-tour').addEventListener('click', () => {
    tour.start();
});