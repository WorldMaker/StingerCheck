requirejs.config({
    baseUrl: '/App/',
    paths: {
        'text': '../Scripts/text',
        'durandal': '../Scripts/durandal',
        'plugins': '../Scripts/durandal/plugins',
        'transitions': '../Scripts/durandal/transitions',
        'bootstrap': '../Scripts/bootstrap',
        'jquery': '../Scripts/jquery-2.1.0',
        'knockout': '../Scripts/knockout-3.1.0',
        'nprogress': '../Scripts/nprogress',
        'toastr': '../Scripts/toastr'
    },
    shim: {
        'bootstrap': ['jquery'],
        'jquery': {
            exports: '$'
        },
        'nprogress': {
            deps: ['jquery'],
            exports: 'NProgress'
        }
    }
});

define(['durandal/system', 'durandal/app', 'durandal/viewLocator', 'bootstrap'],  function (system, app, viewLocator) {
    //>>excludeStart("build", true);
    system.debug(true);
    //>>excludeEnd("build");

    app.title = 'Stinger Check';

    app.configurePlugins({
        router: true,
        //dialog: true,
        widget: {
            kinds: ['disqus']
        }
    });

    app.start().then(function() {
        //Replace 'viewmodels' in the moduleId with 'views' to locate the view.
        //Look for partial views in a 'views' folder in the root.
        viewLocator.useConvention();

        //Show the app by setting the root view model for our application with a transition.
        app.setRoot('viewmodels/shell', 'entrance');
    });
});