# CoffeeScript

Site = angular.module 'fp-wiki', []

Site.factory 'SearchApi', ['$resource', ($resource) -> $resource 'api/Search/']