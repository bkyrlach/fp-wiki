# CoffeeScript

Site.controller 'SearchController', ['$scope', 'SearchApi', ($scope, searchApi) ->
  $scope.greeting = 'Hello, world'
  $scope.query = ''
  $scope.methods = null
  $scope.details = null
  $scope.search = () -> 
    $scope.methods = null
    searchApi.get { search: $scope.query } , 
      (result) -> 
        $scope.methods = result
        console.log($scope.methods)

  $scope.goToDetails = (method) ->
    searchApi.get {id: method.Id} ,
      (result) ->
        $scope.details = result

  $scope.resetDetails = () ->
    $scope.details = null
  true
]