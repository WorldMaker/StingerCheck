﻿module.exports = function (grunt) {
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),
        typescript: {
            base: {
                src: ['**/*.ts'],
                options: {
                    module: 'amd', //or commonjs
                    target: 'es5', //or es3
                    sourcemap: true,
                }
            }
        },
        durandal: {
            dist: {
                src: [
                    "app/**/*.js",
                    "app/**/*.html",
                    "scripts/bootstrap.js",
                    "scripts/durandal/**/*.*",
                    "scripts/jquery-2.1.0.js",
                    "scripts/knockout-3.1.0.js",
                    "scripts/knockout.validation.js",
                    "scripts/nprogress.js",
                    "scripts/toastr.js",
                    "scripts/underscore-ko.js"
                ],
                options: {
                    baseUrl: "app/",
                    mainPath: "app/main.js",
                    out: "app/main-built.js",
                    paths: {
                        'bootstrap': '../Scripts/bootstrap',
                        'jquery': '../Scripts/jquery-2.1.0',
                        'knockout': '../Scripts/knockout-3.1.0',
                        'knockout.validation': '../Scripts/knockout.validation',
                        'nprogress': '../Scripts/nprogress',
                        'toastr': '../Scripts/toastr',
                        'underscore-ko': '../Scripts/underscore-ko'
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
                        }
                    }
                }
            }
        }
    });

    grunt.loadNpmTasks('grunt-typescript');
    grunt.loadNpmTasks('grunt-durandal');

    grunt.registerTask('default', ['typescript', 'durandal']);
}