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
    text: 'Use this button to show/hide side navigation',
    attachTo: {
        element: '#step1',
        on: 'bottom'
    },
    buttons: [
        {
            text: 'Next',
            action: tour.next,
            classes: 'shepherd-button shepherd-button-primary'
        }
    ]
});

tour.addStep({
    id: 'step2',
    text: 'Use this button to view/hide your profile information',
    attachTo: {
        element: '#step2',
        on: 'bottom'
    },
    buttons: [
        {
            text: 'Back',
            action: tour.back,
            classes: 'shepherd-button shepherd-button-secondary'
        },
        {
            text: 'Next',
            action: tour.next,
            classes: 'shepherd-button shepherd-button-primary'
        },
        {
            text: 'Finish',
            action: tour.complete,
            classes: 'shepherd-button shepherd-button-primary'
        }
    ]
});

tour.addStep({
    id: 'step3',
    text: 'Use this button to log out',
    attachTo: {
        element: '#step3',
        on: 'bottom'
    },
    buttons: [
        {
            text: 'Back',
            action: tour.back,
            classes: 'shepherd-button shepherd-button-secondary'
        },
        {
            text: 'Next',
            action: tour.next,
            classes: 'shepherd-button shepherd-button-primary'
        },
        {
            text: 'Finish',
            action: tour.complete,
            classes: 'shepherd-button shepherd-button-primary'
        }
    ]
});

tour.addStep({
    id: 'step4',
    text: 'Use this button to change light/dark theme',
    attachTo: {
        element: '#step4',
        on: 'bottom'
    },
    buttons: [
        {
            text: 'Back',
            action: tour.back,
            classes: 'shepherd-button shepherd-button-secondary'
        },
        {
            text: 'Next',
            action: tour.next,
            classes: 'shepherd-button shepherd-button-primary'
        },
        {
            text: 'Finish',
            action: tour.complete,
            classes: 'shepherd-button shepherd-button-primary'
        }
    ]
});

tour.addStep({
    id: 'step5',
    text: 'Use this button to enable/disable full screen mode',
    attachTo: {
        element: '#step5',
        on: 'bottom'
    },
    buttons: [
        {
            text: 'Back',
            action: tour.back,
            classes: 'shepherd-button shepherd-button-secondary'
        },
        {
            text: 'Next',
            action: tour.next,
            classes: 'shepherd-button shepherd-button-primary'
        },
        {
            text: 'Finish',
            action: tour.complete,
            classes: 'shepherd-button shepherd-button-primary'
        }
    ]
});

tour.addStep({
    id: 'step6',
    text: 'Use this dropdown to switch language',
    attachTo: {
        element: '#step6',
        on: 'bottom'
    },
    buttons: [
        {
            text: 'Back',
            action: tour.back,
            classes: 'shepherd-button shepherd-button-secondary'
        },
        {
            text: 'Next',
            action: tour.next,
            classes: 'shepherd-button shepherd-button-primary'
        },
        {
            text: 'Finish',
            action: tour.complete,
            classes: 'shepherd-button shepherd-button-primary'
        }
    ]
});

tour.addStep({
    id: 'step7',
    text: 'Use this dropdown to switch currency',
    attachTo: {
        element: '#step7',
        on: 'bottom'
    },
    buttons: [
        {
            text: 'Back',
            action: tour.back,
            classes: 'shepherd-button shepherd-button-secondary'
        },
        {
            text: 'Next',
            action: tour.next,
            classes: 'shepherd-button shepherd-button-primary'
        },
        {
            text: 'Finish',
            action: tour.complete,
            classes: 'shepherd-button shepherd-button-primary'
        }
    ]
});

tour.addStep({
    id: 'step8',
    text: 'Use this button to restart the tour',
    attachTo: {
        element: '#step8',
        on: 'bottom'
    },
    buttons: [
        {
            text: 'Back',
            action: tour.back,
            classes: 'shepherd-button shepherd-button-secondary'
        },
        {
            text: 'Next',
            action: tour.next,
            classes: 'shepherd-button shepherd-button-primary'
        },
        {
            text: 'Finish',
            action: tour.complete,
            classes: 'shepherd-button shepherd-button-primary'
        }
    ]
});

tour.addStep({
    id: 'step9',
    text: 'Use the sidebar for quick navigation around the work panel',
    attachTo: {
        element: '#step9',
        on: 'bottom'
    },
    buttons: [
        {
            text: 'Back',
            action: tour.back,
            classes: 'shepherd-button shepherd-button-secondary'
        },
        {
            text: 'Next',
            action: tour.next,
            classes: 'shepherd-button shepherd-button-primary'
        },
        {
            text: 'Finish',
            action: tour.complete,
            classes: 'shepherd-button shepherd-button-primary'
        }
    ]
});

tour.addStep({
    id: 'step10',
    text: 'Have fun!',
    attachTo: {
        element: '#step10',
        on: 'bottom'
    },
    buttons: [
        {
            text: 'Back',
            action: tour.back,
            classes: 'shepherd-button shepherd-button-secondary'
        },
        {
            text: 'Finish',
            action: tour.complete,
            classes: 'shepherd-button shepherd-button-primary'
        }
    ]
});

function startTour() {
    tour.start();
}