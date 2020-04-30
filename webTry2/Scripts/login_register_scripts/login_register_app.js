var app = angular.module("loginRegister", []);
app.controller("login_register_controller", function ($scope, $http) {

    $scope.User = {
        userName: "",
        password: "",
        firstName: null,
        lastName: null,
        email: null,
        phoneNumber: "",
        permission: "",
        unit: ""
    };


    $scope.checkUserLogin = function () {
        //method : post / get , data : what data we want to send , url: in C# controller-> what function we want
        console.log("in check angular");
       
        $http({
            method: "POST",
            data: { userObj: $scope.User },
            //url define what function we applay to when post to the server
            url: "Home/LoginCheck"
        }).then(function (response) {
            //
            // move on to the next page
            let user = response.data;
            if (user != "") {
                var queryString = "?userName=" + user.userName;
                if (user.permission == "employee")
                    window.location.href = "/Employee/employeePage" + queryString;
                else if (user.permission == "manager")
                    window.location.href = "/Admin/adminPage" + queryString;
            }
            else {
                alert("please check your user name or password, if you dont have accout yet please register");
            }
        });
    };


    //for registration
    $scope.userRegister = function () {
        //post request for registration with user data
        console.log("registration check - user details");
            $http({
                method: "POST",
                data: $scope.User,
                //url define what function we applay to when post to the server
                url: "Home/userRegistration"
            }).then(function (dataReturn) {
                if (dataReturn.data == "") {
                    alert("ERROR: check your user name, there is identical user name on DB");
                    console.log("registration fail");
                }
                else {
                    $('#loginbox').show();
                    $('#signupbox').hide();
                    console.log("registration success");
                }
            });
    };

     /* checking if user have allready use "save password"
        if ($scope.User.UserName == "") {
            if ($('#login-username').val() != "") {
                $scope.User.UserName = $('#login-username').val();
                $scope.User.password = $('#login-password').val();
            } else {
                alert("please check your user name or password");
            } 
        }*/

    /* for sharing between controllers in the same app 
        app.factory("sharedData", function () {
            var dataSharing = [];
            var addData = function (key, value) {
                var data = {
                    key: key,
                    value: value
                };
                context.push(data);
            }
            var getData = function (key) {
                var data = _.find(context, {
                    key: key
                });
                return data;
            }
    
            return {
                addData: addData,
                getData: getData
            }
        });*/

});