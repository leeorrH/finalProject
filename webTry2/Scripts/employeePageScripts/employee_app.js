﻿
var app = angular.module("employeeAng", []);
app.controller("employeePageContoller", function ($scope, $http, $location, $timeout) {

    var postData = $location.$$absUrl.split("?");
    $scope.userName = postData[1].split("=");
    $scope.userName = $scope.userName[1];

    //for visibility component in employee report - hide or visable
    $scope.deliverToEmpDetails = true;
    $scope.attachReference = true;
    $scope.locationDetails = true;
    $scope.EncChangeStatus = true;
    $scope.approveInUse = true;
    $scope.ReasonToUpdate = true;
    //    for EDIT and Display
    /* introduction: userEncryptors including encryptors locations- lat and long!
     * markers init function: initMarkers without setting them on map
     * set Markers on map function: setMarkersOnMap
     */
    $scope.userEncryptors = [];
    var markesrArray = [];
    $scope.gMap;

    //user basic deatails
    $scope.UserObj = {
        "UserName": "",
        "firstName": "",
        "lastName": "",
        "email": "",
        "phoneNumber": ""
    };

    //for EDIT window
    $scope.employees = []; //array of employees
    $scope.buildings = []; //array of buildings
    $scope.floors = []; //array of floors
    $scope.rooms = []; //array of rooms

    $scope.statusArray = []; // need to have all posible status
    $scope.sitesLocationsArray = []; // have all locations buildings and coordinates
    
    //variables for showing data on edit window
    $scope.tempDataForEditWindow;
    var tempDataIndex;

    /*              controller functions                   */

    /* on load - getting all user encryptors by his user number - userName
     * also get user details */
    $scope.onLoad = function () {
        //getting encryptors
        $http({
            method: "POST",
            data: { "userName": $scope.userName },
            //url define what function we apply to when post to the server
            //url: "loadEmployeeEncryptors"
            url: "loadEmployeeEncryptors"
        }).then(function (dataReturn) {
            var data = dataReturn.data;

            $.each(data, function (index, record) {
                $scope.userEncryptors.push(record);
            });
             //loading map object
            $scope.initialize(); 
           // $('selectStatus option[value=' + $scope.userEncryptors + "']").attr("selected", "selected");
        });
        //getting user details  
    };

    //function for display and update 
    $scope.getRowData = function (index) {
        $scope.tempDataForEditWindow = $scope.userEncryptors[index];
        //select initialize 
        switch ($scope.userEncryptors[index].status) {
            case "in use":
                $('#statusSelect').val('1');
            case "destroyed":
                $('#statusSelect').val('2');
            case "lost":
                $('#statusSelect').val('3');
            case "delivered": 
                $('#statusSelect').val('4');
            default: 
                $('#statusSelect').val('1');
        }
        tempDataIndex = index;
    }; 

    $scope.logout = function () {
       window.location.href = "/";
    };

    //report function:
    $scope.cleanReport = function () {


    };

    $scope.ReportAbout = function () {
        var reason = $scope.SelectesReasonReport
        switch (reason) {
            case 'monthly report':
                $scope.approveInUse = false;
                $scope.deliverToEmpDetails = true;
                $scope.attachReference = true;
                $scope.locationDetails = true;
                $scope.EncChangeStatus = true;
                $scope.ReasonToUpdate = true;
                break;
            case 'changing encryptor location':
                $scope.approveInUse = true;
                $scope.locationDetails = false;
                $scope.deliverToEmpDetails = true;
                $scope.attachReference = true;
                $scope.EncChangeStatus = true;
                $scope.ReasonToUpdate = false;
                break;
            case 'changing encryptor status':
                $scope.approveInUse = true;
                $scope.deliverToEmpDetails = true;
                $scope.attachReference = false;
                $scope.locationDetails = true;
                $scope.EncChangeStatus = false;
                $scope.ReasonToUpdate = false;
                break;
            case 'deliver to employee':
                $scope.approveInUse = true;
                $scope.locationDetails = false;
                $scope.deliverToEmpDetails = false;
                $scope.attachReference = true;
                $scope.EncChangeStatus = true;
                $scope.ReasonToUpdate = false;
                $scope.employees = []; 
                getAllEmployees();
                break;   
        }
    };


    function getAllEmployees() {
        //getting employees
        $http({
            method: "POST",
            data: { "empUserName": $scope.userName } ,
            url: "getEmployeeList"
        }).then(function (dataReturn) {

            if (dataReturn.data == "") {
                alert("no employee return");
                console.log("get employees maybe fail");
                return;
            }
            var data = dataReturn.data;

            $.each(data, function (index, emp) {
                $scope.employees.push(emp);
            });

            $timeout(function () {
                    $('.selectpicker').selectpicker('refresh'); //put it in timeout for run digest cycle
                }, 1)
            .catch(() => {
                console.log('nope');
            });
        });
    }

    $scope.getEmpValue = function () {
        var employeeUserName = $scope.empID;
    };

    $scope.getBuildings = function () {
        var x = $scope.siteName;
        if ($scope.siteName == "") {
            $scope.buildings = "";
        }
        else {
            //getting building
            $http({
                method: "POST",
                data: { "siteName": $scope.siteName },
                url: "getBuildingList"
            }).then(function (dataReturn) {

                if (dataReturn.data == "") {
                    // alert("no building return");
                    console.log("get buildings maybe fail");
                    return;
                }
                $scope.buildings = dataReturn.data;

                $timeout(function () {
                    $('.selectpicker').selectpicker('refresh'); //put it in timeout for run digest cycle
                }, 1)
                    .catch(() => {
                        console.log('nope');
                    });
            });
        }
    };

    $scope.getfloors = function () {
        //getting floors
        $http({
            method: "POST",
            data: { "siteName": $scope.siteName, "buildName": $scope.buildName },
            url: "getfloorsList"
        }).then(function (dataReturn) {

            if (dataReturn.data == "") {
                //alert("no floor return");
                console.log("get floors maybe fail");
                return;
            }
            $scope.floors = dataReturn.data;

            $timeout(function () {
                $('.selectpicker').selectpicker('refresh'); //put it in timeout for run digest cycle
            }, 1)
                .catch(() => {
                    console.log('nope');
                });
        });
    };

    $scope.getroom = function () {
        //getting room
        $http({
            method: "POST",
            data: { "siteName": $scope.siteName, "buildName": $scope.buildName, "floorNumber": $scope.floorNumber },
            url: "getRoomList"
        }).then(function (dataReturn) {

            if (dataReturn.data == "") {
                alert("no room return");
                console.log("get rooms maybe fail");
                return;
            }
            $scope.rooms = dataReturn.data;

            $timeout(function () {
                $('.selectpicker').selectpicker('refresh'); //put it in timeout for run digest cycle
            }, 1)
                .catch(() => {
                    console.log('nope');
                });
        });
    };

    /*      map view page , second page for employee        */
    $scope.searchResults = function () {
        //continue with searching ! 
        var searchIn = $('#searchInput').val();
        var result = $scope.userEncryptors.includes(searchIn);
        Console.log(result);
    };

    $scope.initialize = function () {
        var googleMapOption = {
            zoom: 6.99,
            center: new google.maps.LatLng(32.3571742, 36.9767603),
            mapTypeId: google.maps.MapTypeId.TERRAIN,
            mapTypeControl: true,
            streetViewControl: false
        };

        $scope.gMap = new google.maps.Map(document.getElementById('googleMap'), googleMapOption);

        initMarkers();//init markers on map
        clusterMarkers();

    };

    /* introduction: init maps create markers on map while taking lat and lng from
     * userEncryptors object
     * also push markers to markesrArray */
    function initMarkers() {
        angular.forEach($scope.userEncryptors, function (encryptor, index) {
            var marker = new google.maps.Marker({
                //setting marker position
                position: new google.maps.LatLng(encryptor.deviceLocation.latitude, encryptor.deviceLocation.longitude),
                map: $scope.gMap
            });
            markesrArray.push(marker);
        });
    };

    /* introduction: clusterMarkers function cluster marker by their location 
       markerClusterer on google */
    function clusterMarkers() {
        var markerCluster = new MarkerClusterer($scope.gMap, markesrArray,
            { imagePath: 'https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/m' });
    };

});


/*
app.factory('dataSharing', function () {
    var data = {
        encryptorsArray: {}
    };
})*/