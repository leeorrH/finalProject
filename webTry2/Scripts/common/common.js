angular.module('commonMod', []).factory('commonFunctions', function ($http) {

    return {
        getRequests : function (userName, permis) {
            var reqs = [];
            $http({
                method: "POST",
                data: { "userName": userName, "permission": permis },
                url: "getRequests"
            }).then(function (dataReturn) {
                var data = dataReturn.data;

                $.each(data, function (index, record) {

                });
            });
            return;
        },

        getUserDetails : function(userName) {
            return $http({
                method: "POST",
                data: { 'userName': userName },
                url: "getUserDetails"
            });
        }

    }
    
});