module.exports = function (grunt) {
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),
        durandal: {
            dist: {
                src: [
                    "app/**/*.js",
                    "app/**/*.html",
                    "scripts/bootstrap.js",
                    "scripts/durandal/**/*.*",
                    "scripts/jquery-2.1.0.js",
                    "scripts/knockout-3.1.0.js",
                    "scripts/nprogress.js",
                    "scripts/toastr.js",
                ],
                options: {
                    baseUrl: "app/",
                    mainPath: "app/main.js",
                    out: "app/main-built.js",
                    paths: {
                        'bootstrap': '../Scripts/bootstrap',
                        'jquery': '../Scripts/jquery-2.1.0',
                        'knockout': '../Scripts/knockout-3.1.0',
                        'nprogress': '../Scripts/nprogress',
                        'toastr': '../Scripts/toastr',
                    },
                    shim: {
                        'bootstrap': ['jquery'],
                        'jquery': {
                            exports: "$"
                        },
                        'nprogress': {
                            deps: ['jquery'],
                            exports: 'NProgress'
                        }
                    },

                    uglify2: {
                        compress: {
                            global_defs: {
                                DEBUG: false
                            }
                        },
                        sourceMap: true
                    }
                }
            }
        }
    });

    grunt.loadNpmTasks('grunt-durandal');

    grunt.registerTask('default', ['durandal']);
}