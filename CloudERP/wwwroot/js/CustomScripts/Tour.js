const tour = new Shepherd.Tour({
    defaultStepOptions: {
        classes: 'shadow-md bg-purple-dark',
        scrollTo: true,
        cancelIcon: {
            enabled: true
        }
    }
});

function initializeTour() {
    tour.steps = [];

    tour.addStep({
        id: 'step1',
        text: window.localizedSteps.guide.step1,
        attachTo: {
            element: '#step1',
            on: 'bottom'
        },
        buttons: [
            {
                text: window.localizedSteps.guide.next,
                action: tour.next,
                classes: 'shepherd-button shepherd-button-primary'
            }
        ]
    });

    tour.addStep({
        id: 'step2',
        text: window.localizedSteps.guide.step2,
        attachTo: {
            element: '#step2',
            on: 'bottom'
        },
        buttons: [
            {
                text: window.localizedSteps.guide.back,
                action: tour.back,
                classes: 'shepherd-button shepherd-button-secondary'
            },
            {
                text: window.localizedSteps.guide.next,
                action: tour.next,
                classes: 'shepherd-button shepherd-button-primary'
            },
            {
                text: window.localizedSteps.guide.finish,
                action: tour.complete,
                classes: 'shepherd-button shepherd-button-primary'
            }
        ]
    });

    tour.addStep({
        id: 'step3',
        text: window.localizedSteps.guide.step3,
        attachTo: {
            element: '#step3',
            on: 'bottom'
        },
        buttons: [
            {
                text: window.localizedSteps.guide.back,
                action: tour.back,
                classes: 'shepherd-button shepherd-button-secondary'
            },
            {
                text: window.localizedSteps.guide.next,
                action: tour.next,
                classes: 'shepherd-button shepherd-button-primary'
            },
            {
                text: window.localizedSteps.guide.finish,
                action: tour.complete,
                classes: 'shepherd-button shepherd-button-primary'
            }
        ]
    });

    tour.addStep({
        id: 'step4',
        text: window.localizedSteps.guide.step4,
        attachTo: {
            element: '#step4',
            on: 'bottom'
        },
        buttons: [
            {
                text: window.localizedSteps.guide.back,
                action: tour.back,
                classes: 'shepherd-button shepherd-button-secondary'
            },
            {
                text: window.localizedSteps.guide.next,
                action: tour.next,
                classes: 'shepherd-button shepherd-button-primary'
            },
            {
                text: window.localizedSteps.guide.finish,
                action: tour.complete,
                classes: 'shepherd-button shepherd-button-primary'
            }
        ]
    });

    tour.addStep({
        id: 'step5',
        text: window.localizedSteps.guide.step5,
        attachTo: {
            element: '#step5',
            on: 'bottom'
        },
        buttons: [
            {
                text: window.localizedSteps.guide.back,
                action: tour.back,
                classes: 'shepherd-button shepherd-button-secondary'
            },
            {
                text: window.localizedSteps.guide.next,
                action: tour.next,
                classes: 'shepherd-button shepherd-button-primary'
            },
            {
                text: window.localizedSteps.guide.finish,
                action: tour.complete,
                classes: 'shepherd-button shepherd-button-primary'
            }
        ]
    });

    tour.addStep({
        id: 'step6',
        text: window.localizedSteps.guide.step6,
        attachTo: {
            element: '#step6',
            on: 'bottom'
        },
        buttons: [
            {
                text: window.localizedSteps.guide.back,
                action: tour.back,
                classes: 'shepherd-button shepherd-button-secondary'
            },
            {
                text: window.localizedSteps.guide.next,
                action: tour.next,
                classes: 'shepherd-button shepherd-button-primary'
            },
            {
                text: window.localizedSteps.guide.finish,
                action: tour.complete,
                classes: 'shepherd-button shepherd-button-primary'
            }
        ]
    });

    tour.addStep({
        id: 'step7',
        text: window.localizedSteps.guide.step7,
        attachTo: {
            element: '#step7',
            on: 'bottom'
        },
        buttons: [
            {
                text: window.localizedSteps.guide.back,
                action: tour.back,
                classes: 'shepherd-button shepherd-button-secondary'
            },
            {
                text: window.localizedSteps.guide.next,
                action: tour.next,
                classes: 'shepherd-button shepherd-button-primary'
            },
            {
                text: window.localizedSteps.guide.finish,
                action: tour.complete,
                classes: 'shepherd-button shepherd-button-primary'
            }
        ]
    });

    tour.addStep({
        id: 'step8',
        text: window.localizedSteps.guide.step8,
        attachTo: {
            element: '#step8',
            on: 'bottom'
        },
        buttons: [
            {
                text: window.localizedSteps.guide.back,
                action: tour.back,
                classes: 'shepherd-button shepherd-button-secondary'
            },
            {
                text: window.localizedSteps.guide.next,
                action: tour.next,
                classes: 'shepherd-button shepherd-button-primary'
            },
            {
                text: window.localizedSteps.guide.finish,
                action: tour.complete,
                classes: 'shepherd-button shepherd-button-primary'
            }
        ]
    });

    tour.addStep({
        id: 'step9',
        text: window.localizedSteps.guide.step9,
        attachTo: {
            element: '#step9',
            on: 'bottom'
        },
        buttons: [
            {
                text: window.localizedSteps.guide.back,
                action: tour.back,
                classes: 'shepherd-button shepherd-button-secondary'
            },
            {
                text: window.localizedSteps.guide.next,
                action: tour.next,
                classes: 'shepherd-button shepherd-button-primary'
            },
            {
                text: window.localizedSteps.guide.finish,
                action: tour.complete,
                classes: 'shepherd-button shepherd-button-primary'
            }
        ]
    });

    tour.addStep({
        id: 'step10',
        text: window.localizedSteps.guide.step10,
        attachTo: {
            element: '#step10',
            on: 'bottom'
        },
        buttons: [
            {
                text: window.localizedSteps.guide.back,
                action: tour.back,
                classes: 'shepherd-button shepherd-button-secondary'
            },
            {
                text: window.localizedSteps.guide.finish,
                action: tour.complete,
                classes: 'shepherd-button shepherd-button-primary'
            }
        ]
    });
}

function startTour() {
    if (document.querySelector('#step1')) {
        initializeTour();
        tour.start();
    } else {
        console.warn('Tour cannot start: step1 element not found');
    }
}

function refreshTour() {
    if (tour.isActive()) {
        tour.cancel();
    }
    initializeTour();
    startTour();
}

initializeTour();