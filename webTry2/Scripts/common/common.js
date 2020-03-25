angular.module('commonMod', []).factory('commonFunctions', function ($http) {

    return {
        getReports : function (userName, permis) {
            $http({
                method: "POST",
                data: { "userName": userName, "permission": permis },
                url: "getRequests"
            });
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