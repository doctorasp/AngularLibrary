var app = angular.module('comment', ['angularTrix']);

(function (app) {
    "use strict";

    app.controller('commCtrl', function ($scope, $timeout, $sce, $http, CommentService) {


        $scope.getCommentWall = function getCommentWall(bookId) {
            this.bookId = bookId;
            var getData = CommentService.getCommentWall($scope.bookId);
            getData.then(function (b) {
                $scope.comments = b.data;
            }, function (b) {
                console.log(b);
            });
        }
        function ReloadComments() {
            var getData = CommentService.getCommentWall($scope.bookId);
            getData.then(function (b) {
                $scope.comments = b.data;
            }, function (b) {
                console.log(b);
            });
        }


        $scope.addComment = function () {
            $http.post('/Comments/AddComment', $scope.model).then(
                function (successResponse) {
                    ReloadComments();
                    $scope.model = {};
                    $scope.Comment.$setPristine();
                    $scope.Comment.$setUntouched();
                    console.log('success');
                },
                function (errorResponse) {
                    // handle errors here
                });
        }
    });

})(app);

app.filter('unsafe', function ($sce) { return $sce.trustAsHtml; });

app.service('CommentService', function ($http) {
    this.getCommentWall = function(id){
        return $http.get('/Comments/CommentWall/?id='+id);
    };

   
});
angular.bootstrap(document.getElementById("comment"), ['comment']);