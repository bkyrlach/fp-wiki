# CoffeeScript

Site.controller 'SearchController', ['$scope', 'SearchApi', 'HelpApi', ($scope, searchApi, helpApi) ->  
  $scope.query = ''
  $scope.results = null
  $scope.details = null
  $scope.editMode = false
  $scope.search = () ->     
    $scope.details = null
    searchApi.get { search: $scope.query } , 
      (result) ->
        $scope.results = result
        true
    true    

  $scope.goToDetails = (helpId) ->
    helpApi.get {id: helpId} ,
      (result) ->
        $scope.details = result
        hljs.initHighlighting()
        true
    true

  $scope.edit = () ->
    $scope.editMode = true
    true

  $scope.done = () ->
    $scope.editMode = false
    helpApi.save 
        id: $scope.details.helpId
        blurb: $scope.details.blurb
        helpContent: $scope.details.content
    true

  $scope.resetDetails = () ->
    $scope.details = null
  true
]