
﻿var app = angular.module("MyApp", ['app', 'comment']);
app.controller("myCtrl", function ($scope, BookService) {
    $scope.search = '';
    $scope.sortType = 'Name';
    $scope.sortReverse = true;
    $scope.term='';
    getAllBooks();
    getAllFiles();
    getAllGanres();
    getRecentComments();



    $scope.removeBook = function (index) {
        BookService.deleteBook(index);
        location.reload();
    };

    $scope.removeGenre = function (index) {
        BookService.deleteGenre(index);
        location.reload();
    };


    function ReloadBooks() {
        var getData = BookService.getAllBooks();
        getData.then(function (b) {
            $scope.books = b.data;
        }, function (b) {
            console.log(b);
        });
    }

    function getAllFiles() {
        var getData = BookService.getFiles();
        getData.then(function (b) {
            $scope.link = b.data;
        }, function (b) {
            console.log(b);
        });
    }

    $scope.searchName = function (value, index) {
        if (value == '') {
            $scope.search = '';
        }
        else {
            $scope.search = { GenreName: value };
            var el = document.getElementById("search");
            el.value = '';
        }
        $scope.selected = index;
        var el = document.getElementById("mainTab");
        el.classList.remove("active");
    }
 
    function getAllGanres() {
        var getData = BookService.getGanres();
        getData.then(function (g) {
            $scope.genres = g.data;
        }, function (g) {
            console.log(g)
        });
    }

    function getRecentComments() {
        var getData = BookService.getRecentComments();
        getData.then(function (c) {
            $scope.rcomm = c.data;
        }, function (c) {
            console.log(c);
        })
    }

    $scope.similarBooks = function getSimilarBooks(term) {
        this.term = term;
        var getData = BookService.getSimilarBooks($scope.term);
        getData.then(function (b) {
            $scope.similars = b.data;
        }, function (b) {
            console.log(b);
        });
    }

    function getAllBooks() {
        var getData = BookService.getBooks();

        getData.then(function(b) {
            $scope.books = b.data;
            $scope.filteredItems = $scope.books.length;
            $scope.totalItems = $scope.books.length;
            
            $scope.PerPageItems = 6;
            $scope.currentPage = 1;

        }, function(b) {
            alert("fail");
            });

        $scope.filter = function () {
            $timeout(function () {
                $scope.filteredItems = $scope.filtered.length;
            }, 10);
        };

        $scope.numberOfPages = function () {
            return Math.ceil($scope.totalItems / $scope.PerPageItems);
        } 

        $scope.convert = function _arrayBufferToBase64(buffer) {
            var binary = '';
            var bytes = new Uint8Array(buffer);
            var len = bytes.byteLength;
            for (var i = 0; i < len; i++) {
                binary += String.fromCharCode(bytes[i]);
            }
            return window.btoa(binary);
        }

        $scope.getType = function getType(str) {
            return str.substring(str.length - 3);
        }

        $scope.toBase64 = function _arrayBufferToBase64(buffer) {
            var binary = '';
            var bytes = new Uint8Array(buffer);
            var len = bytes.byteLength;
            for (var i = 0; i < len; i++) {
                binary += String.fromCharCode(bytes[i]);
            }
            return window.btoa(binary);
        }

        $scope.filter = function () {
            $timeout(function () {
                $scope.filteredItems = $scope.filtered.length;
            }, 10);
        };

        $scope.numberOfPages = function () {
            return Math.ceil($scope.totalItems / $scope.PerPageItems);
        } 
    }
});

app.directive('ngConfirmClick', [
    function () {
        return {
            link: function (scope, element, attr) {
                var msg = attr.ngConfirmClick || "Are you sure?";
                var clickAction = attr.confirmedClick;
                element.bind('click', function (event) {
                    if (window.confirm(msg)) {
                        scope.$eval(clickAction)
                    }
                });
            }
        };
    }])


app.filter('unsafe', function ($sce) {
    return $sce.trustAsHtml;
});

app.filter('Pagestart', function () {
    return function (input, start) {
        if (!input || !input.length) { return; }
        start = +start; //parse to int
        return input.slice(start);
    }
});    

app.service("BookService", function ($http) {

    this.getBooks = function () {
        return $http.get("/Books/GetBooksAsync");
    };

    this.getFiles = function () {
        return $http.get("/Books/GetFilePath/");
    };

    this.getSimilarBooks = function (term) {
        return $http.get("/Books/GetSimilarBooks/?term=" + term);
    };

    this.getGanres = function () {
        return $http.get("/Books/GetGanres/");
    };

    this.getRecentComments = function () {
        return $http.get("Comments/GetRecentComments/");
    };

    this.deleteBook = function (index) {
        return $http.post('Books/Delete/' + index);
    };

    this.deleteGenre = function (index) {
        return $http.post('Genres/Delete/' + index);
    };
});
