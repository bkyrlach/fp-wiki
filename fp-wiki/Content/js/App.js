// Generated by CoffeeScript 1.10.0
var Site;

Site = angular.module('fp-wiki', []);

Site.factory('SearchApi', [
  '$resource', function($resource) {
    return $resource('api/Search/');
  }
]);
