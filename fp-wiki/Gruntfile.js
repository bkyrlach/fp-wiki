﻿module.exports = function (grunt) {

  /* grunt initialize configuration */
  grunt.initConfig({
    /* where your package manager lives */
    pkg: grunt.file.readJSON('package.json'),

    /* configuration for bower to include bower installed js libraries */
    /* grunt bower */
    bower: {
      solution: {
        dest: 'fp-wiki/content/vendor/',
        css_dest: 'fp-wiki/content/vendor/css/',
          fonts_dest: 'Audi.Bridge.Webfp-wiki/content/vendor/fonts/',
          js_dest: 'fp-wiki/content/vendor/scripts/',
        options: {
          keepExpandedHierarchy: false,
          packageSpecific: {
            'bootstrap': {
              files: ['dist/css/bootstrap.css', 'dist/fonts/*', 'dist/js/bootstrap.js']
            },
            'codemirror': {
              files: ['lib/codemirror.js', 'lib/codemirror.css', 'addon/edit/matchbrackets.js', 'mode/css/css.js', 'mode/htmlmixed/htmlmixed.js', 'mode/javascript/javascript.js', 'mode/xml/xml.js']
            }
          }
        }
      }
    },

    /* configuration for cleaning up grunt created files */
    /* grunt clean */
    clean: {
      solution: ['**/content/*.css', '**/content/scripts', '**/content/templates', '**/content/vendor', '**/content/scripts/compile/*.coffee']
    },

    /* configuration for compiling coffeescript files */
    /* grunt coffee */
    coffee: {
      options: {
        sourceMap: false,
        bare: true,
        force: true
      },
      solution: {
        expand: true,
        cwd: 'Coffee',
        src: '**/*.coffee',
        dest: 'Content/js',
        ext: '.js'
      }
    },

    /* configuration for coffeelint */
    /* grunt coffeelint */
    coffeelint: {
      options: {
        configFile: 'coffeelint.json'
      },
      solution: ['fp-wiki/content/**/*.coffee'],
    },

    /* configuration for re-compiling coffeescript/less files after they have been modified */
    /* grunt watch */
    watch: {
      scripts: {
        files: '**/*.coffee',
        tasks: ['coffee'],
        options: {
          spawn: false
        }
      }
    }
  });

    /* load node package manager tasks */
  grunt.loadNpmTasks('grunt-bower');
  grunt.loadNpmTasks('grunt-coffeelint');
  grunt.loadNpmTasks('grunt-contrib-clean');
  grunt.loadNpmTasks('grunt-contrib-coffee');
  grunt.loadNpmTasks('grunt-contrib-watch');
  grunt.loadNpmTasks('grunt-include-source');

    
  grunt.registerTask('compile-coffee', ['coffee', 'bower']);

};