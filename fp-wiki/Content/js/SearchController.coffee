# CoffeeScript

Site.controller 'SearchController', ['$scope', 'SearchApi', ($scope, searchApi) ->
  $scope.greeting = 'Hello, world'
  $scope.query = ''
  $scope.search = () -> searchApi.get { query: $scope.query } , (result) -> console.log(result)
  true
]