var editApp = angular.module("EditApp", []);
editApp.controller("EditCtrl", function ($scope) {

    $scope.deleteFromServer = function deleteFromServer(id) {
        $http.post("/Books/DeleteBookFromServer?id=" + id);
        return true;
    }
});