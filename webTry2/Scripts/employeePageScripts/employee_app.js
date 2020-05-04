
var app = angular.module("employeeAng", ['commonMod']);
app.controller("employeePageContoller", ['$scope', '$location', '$http', '$timeout', 'commonFunctions', function ($scope, $location, $http, $timeout, commonFunctions) {

    var postData = $location.absUrl().split("?");
    $scope.userName = postData[1].split("=");
    $scope.userName = $scope.userName[1];

    //for visibility component in employee report - hide or visable
    $scope.deliverToEmpDetails = true;
    $scope.attachReference = true;
    $scope.locationDetails = true;
    $scope.EncChangeStatus = true;
    $scope.approveInUse = true;
    $scope.ReasonToUpdate = true;
  
    /* introduction: userEncryptors including encryptors locations- lat and long!
     * markers init function: initMarkers without setting them on map
     * set Markers on map function: setMarkersOnMap
     */
    $scope.userEncryptors = [];
    $scope.userReports = [];
    var markersArray = [];
    var historyMarkers = []; 
    //history lines
    var coordinatesArray = [];
    var line;
    //$scope.gMap;
    var markerCluster;

    //user basic deatails
    var userDetails;
    

    //for EDIT window
    $scope.employees = []; //array of employees
    $scope.buildings = []; //array of buildings
    $scope.floors = []; //array of floors
    $scope.rooms = []; //array of rooms

    $scope.statusArray = []; // need to have all posible status
    $scope.sitesLocationsArray = []; // have all locations buildings and coordinates
    
    //variables for showing data on edit window
    $scope.tempDataForEditWindow;
    var tempDataIndex = 0;

    /*              controller functions                   */

    /* on load - getting all user encryptors by his user number - userName
     * also get user details */
    $scope.onLoad = function () {
        if (localStorage.length == 0) {
            localStorage.setItem($scope.userName , $scope.userName);
        }
        else if (!localStorage.getItem($scope.userName)) {
            localStorage.setItem($scope.userName, $scope.userName);
        }
        //getting user details  
        commonFunctions.getUserDetails(localStorage.getItem($scope.userName)).then(function (dataReturn) {
            if (dataReturn.data) {
                userDetails = dataReturn.data;
                $scope.userDetails = userDetails;
                getUserEncryptors();
                getAllEmployees();
            }
        });
        
    };

    

    function getUserEncryptors() {
        //getting encryptors
        $http({
            method: "POST",
            data: { "userName": userDetails.userName },
            //url define what function we apply to when post to the server
            //url: "loadEmployeeEncryptors"
            url: "loadEmployeeEncryptors"
        }).then(function (dataReturn) {
            var data = dataReturn.data;

            $.each(data, function (index, record) {
                $scope.userEncryptors.push(record);
            });
            //loading map object
            initializeMap();
            // $('selectStatus option[value=' + $scope.userEncryptors + "']").attr("selected", "selected");
        });
    }

    //function for display and update 
    $scope.getRowData = function (index) {
        tempDataIndex = index;
        $scope.tempDataForEditWindow = $scope.userEncryptors[index];
        var ownerReportObj = ($scope.employees).find(emp => emp.userName == $scope.tempDataForEditWindow.ownerID);
        if (angular.isUndefined(ownerReportObj)) {
            ownerReportObj = userDetails;
        }
        $scope.ownerFullName = ownerReportObj.firstName + " " + ownerReportObj.lastName;
        //select initialize 
        switch ($scope.userEncryptors[index].status) {
            case "in use":
                $('#statusSelect').val('1');
                break;
            case "destroyed":
                $('#statusSelect').val('2');
                break;
            case "lost":
                $('#statusSelect').val('3');
                break;
            case "delivered":
                $('#statusSelect').val('4');
                break;
            default:
                $('#statusSelect').val('1');
                break;
        }
    }; 

    $scope.logout = function () {
       window.location.href = "/";
    };

    //report functions:
    $scope.cleanReport = function () {
        cleanReport();
    };

    function cleanReport(){
        $('.selectpicker').selectpicker();
        $('.selectpicker').selectpicker('val', '');
        $('.selectpicker[reference="toReset"]').html('');
        $('.selectpicker').selectpicker('refresh');

        $scope.approveInUse = true;
        $scope.deliverToEmpDetails = true;
        $scope.attachReference = true;
        $scope.locationDetails = true;
        $scope.EncChangeStatus = true;

        $scope.SelectesReasonReport = "";
        $scope.encStatus = "";
        $scope.siteName = "";
        $scope.buildName = "";
        $scope.floorNumber = "";
        $scope.room = "";

        $scope.notification = "";
    }

    $scope.ReportAbout = function (reportReason) {
        var reason = $scope.SelectesReasonReport;
        switch (reason) {
            case 'monthly report':
                $scope.approveInUse = false;
                $scope.deliverToEmpDetails = true;
                $scope.attachReference = true;
                $scope.locationDetails = true;
                $scope.EncChangeStatus = true;
                break;
            case 'changing encryptor location':
                $scope.approveInUse = true;
                $scope.locationDetails = false;
                $scope.deliverToEmpDetails = true;
                $scope.attachReference = true;
                $scope.EncChangeStatus = true;
                break;
            case 'changing encryptor status':
                $scope.approveInUse = true;
                $scope.deliverToEmpDetails = true;
                $scope.attachReference = false;
                $scope.locationDetails = true;
                $scope.EncChangeStatus = false;
                break;
            case 'deliver to employee':
                $scope.approveInUse = true;
                $scope.locationDetails = false;
                $scope.deliverToEmpDetails = false;
                $scope.attachReference = true;
                $scope.EncChangeStatus = true;
                $scope.employees = []; 
                getAllEmployees();
                break;
        }
    };


    function getAllEmployees() {
        if (angular.isDefined($scope.employees) && $scope.employees.length == 0) {
            //getting employees
            $http({
                method: "POST",
                url: "getEmployeeList"
            }).then(function (dataReturn) {

                if (dataReturn.data == "") {
                    alert("no employee return");
                    console.log("get employees maybe fail");
                    return;
                }
                var data = dataReturn.data;

                $.each(data, function (index, emp) {
                    if (emp.userName != userDetails.userName) {
                        $scope.employees.push(emp);
                    }
                });

                $timeout(function () {
                    $('.selectpicker').selectpicker('refresh'); //put it in timeout for run digest cycle
                }, 1)
                    .catch(() => {
                        console.log('nope');
                    });
            });
        }
        
    }


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
   
    $scope.sendReport = function () {
        let emp = $scope.emp;
        let encObj = JSON.parse(JSON.stringify($scope.userEncryptors[tempDataIndex]));
        var report = {
            reportType: "" + $scope.SelectesReasonReport,
            reportID: "-1",
            reportOwner: "" + $scope.userEncryptors[tempDataIndex].ownerID,
            date: null,
            notifications: "" + (($scope.notification == undefined) ? "" : $scope.notification),
            enc: encObj ,
            reference: null,
            approvementStatus: 'waiting for approvment'
        };
        let statusCaseFlag = false;
        
        switch (report.reportType) {
            case 'monthly report':
                // TODO
                break;
            case 'changing encryptor location':
                report.enc.deviceLocation = settingNewLocation(report.enc.deviceLocation);
                break;
            case 'changing encryptor status':
                statusCaseFlag = true;
                report.enc.status = "" + $scope.encStatus;

                var filerReader = new FileReader();
                filerReader.onload = function (event) {
                    var res = event.target.result;
                    report.reference = res;
                    postReport(report); 
                }
                var fileList = document.getElementById('refFile').files;
                filerReader.readAsDataURL(fileList[0]);
                
                break;
            case 'deliver to employee':
                report.enc.ownerID = "" + emp.userName;
                report.enc.deviceLocation = settingNewLocation(report.enc.deviceLocation);
                break;
        }
        if (statusCaseFlag == true) {
            statusCaseFlag = false;
        } else {
            postReport(report);
        }
        
       
    };

    function postReport(report) {
        $http({
            method: "POST",
            data: JSON.stringify({ 'empReport': report }),
            url: "SendReport"
        }).then(function (dataReturn) {
            if (dataReturn.data = "sql success") alert("Report send successfully");
            else alert("something go wrong, please try to send again");
            cleanReport();
        }); 
    }


    function settingNewLocation(deviceLocation) {
        deviceLocation.facility = "" + $scope.siteName;
        deviceLocation.building = "" + $scope.buildName;
        deviceLocation.floor = "" + $scope.floorNumber;
        deviceLocation.room =  "" + $scope.room;
        return deviceLocation;
    }

    
    /*      map view page , second page for employee        */


    function initializeMap() {
        var googleMapOption = {
            zoom: 6.99,
            maxZoom:18,
            center: new google.maps.LatLng(32.3571742, 36.9767603),
            mapTypeId: google.maps.MapTypeId.TERRAIN,
            mapTypeControl: true,
            streetViewControl: false
        };

        $scope.gMap = new google.maps.Map(document.getElementById('googleMap'), googleMapOption);

        initMarkers($scope.userEncryptors, markersArray);//init markers on map
        setMapOnAll($scope.gMap, markersArray);
        clusterMarkers(markersArray);

    };

    /* introduction: init maps create markers on map while taking lat and lng from
     * userEncryptors object
     * also push markers to markersArray */
    function initMarkers(encryptorList , markersList) {
        angular.forEach(encryptorList, function (encryptor, index) {
            var contentString = generateContent(encryptor);
            var marker = new google.maps.Marker({
                //setting marker position
                position: new google.maps.LatLng(encryptor.deviceLocation.latitude, encryptor.deviceLocation.longitude),
                label: '' + index , 
                _details: encryptor
            });
       
            k = markersList.push(marker); // k is count on elements in markers array. 

            var infowindow = new google.maps.InfoWindow({
                content: contentString
            });

            markersList[k - 1].addListener('click', function () {
                infowindow.open($scope.gMap, marker);
            });

            
        });
        clusterMarkers(markersList);
    };

    function generateContent(encryptor) {
        // if there is time  - add user details to owner ID ! 
        let employeesArray = $scope.employees;
        let encOwner = employeesArray.find(employee => employee.userName == encryptor.ownerID);

        if (angular.isUndefined(encOwner)) {
            encOwner = userDetails;
        }

        var content =
            '<div id="content" style="margin-left:5px">' +
            '<h4 style="margin-left:5px" > Encryptor details: </h4>' +
            '<ul>' +
            '<li> <b>Encryptor SN:</b> ' + encryptor.serialNumber + '</il>' +
            '<li> <b>Time stamp:</b> ' + encryptor.timestampAsString + '</il>' +
            '<li> <b>Owner:</b> ' + encryptor.ownerID + ' - ' + encOwner.firstName + ' ' + encOwner.lastName + '</il>' +
            '<li> <b>Status:</b> ' + encryptor.status + '</il>' +
            '<li> <b>Facility:</b> ' + encryptor.deviceLocation.facility + '</il>' +
            '<li> <b>Building:</b> ' + encryptor.deviceLocation.building + '</il>' +
            '<li> <b>Floor:</b> ' + encryptor.deviceLocation.floor + '</il>' +
            '<li> <b>Room:</b> ' + encryptor.deviceLocation.room + '</il>' +
            '</ul>' +
            '</div>';
        return content;
    }

    function initCluster() {
        var options_markerclusterer = {
            gridSize: 20,
            maxZoom: 18,
            zoomOnClick: false,
            imagePath: 'https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/m'
        };
        markerCluster = new MarkerClusterer($scope.gMap, markersArray, options_markerclusterer);
    }
    /* introduction: clusterMarkers function cluster marker by their location 
       markerClusterer on google */
    function clusterMarkers(markersList) {

        if (angular.isUndefined(markerCluster)) {
            initCluster();
        }
        markerCluster.clearMarkers();
        markerCluster.addMarkers(markersList);
        
        google.maps.event.addListener(markerCluster, 'clusterclick', function (cluster) {
            var markers = cluster.getMarkers();
            var infoWindow = new google.maps.InfoWindow();
            var array = [];

            for (i = 0; i < markers.length; i++) {
                array.push(generateContent(markers[i]._details) + '<br>');
            }

            if ($scope.gMap.getZoom() > 15) { // optional: $scope.gMap.getZoom() == markerCluster.getMaxZoom()
                infoWindow.setContent(
                    '<div style="margin-left: 10px">' + 
                    "<h3>" + markers.length + " markers: <h3> <br>" + array);
                infoWindow.setPosition(cluster.getCenter());
                infoWindow.open($scope.gMap);
            }
        });

    };

    $scope.getEncryptorHistory = function () {
        historyMarkers = [];
        let tempList = [];
        commonFunctions.getHistory($scope.mapEnc.serialNumber).then(function (dataReturn) {
            var data = dataReturn.data;
            if (!data) {
                alert("no History For this encryptor");
                return;
            } else {
                $.each(data, function (index, record) {
                    tempList.push(record);
                });
            }
            initMarkers(tempList, historyMarkers);
            var currentEnc = $scope.userEncryptors.find(enc => enc.serialNumber == $scope.mapEnc.serialNumber);
            var currEncMarker = new google.maps.Marker({
                //setting marker position
                position: new google.maps.LatLng(currentEnc.deviceLocation.latitude, currentEnc.deviceLocation.longitude),
                label: '' + historyMarkers.length,
                icon: '/Content/images/icons8-marker-64.png',
                _details: currentEnc
            });
            historyMarkers.push(currEncMarker);
            setMapOnAll(null, markersArray);
            setMapOnAll($scope.gMap, historyMarkers);
            clusterMarkers(historyMarkers);
            setHistoryLines(historyMarkers);
        });
    }

    $scope.returnToEncLocation = function () {
        setMapOnAll(null, historyMarkers);
        setMapOnAll($scope.gMap, markersArray);
        clusterMarkers(markersArray);
        removeHistoryLine();
    }

    function setMapOnAll(map, markersList) {
        for (var i = 0; i < markersList.length; i++) {
            markersList[i].setMap(map);
        }
    }

    function removeHistoryLine() {
        if (angular.isDefined(line)) {
            line.setMap(null);
            line = null;
        }  
    }

    function setHistoryLines(historyMarkers) {
        coordinatesArray = [];
        if (line != null && !angular.isUndefined(line)) {
            line.setMap(null);
        }

        angular.forEach(historyMarkers, function (marker) {
            coordinatesArray.push({
                lat: marker._details.deviceLocation.latitude,
                lng: marker._details.deviceLocation.longitude
            });
        });
        var currentEncLocation = markersArray.find(marker => marker._details.serialNumber == $scope.mapEnc.serialNumber)._details;
        if (angular.isDefined(currentEncLocation)) {
            coordinatesArray.push({
                lat: currentEncLocation.deviceLocation.latitude,
                lng: currentEncLocation.deviceLocation.longitude
            });
        }
        
        line = new google.maps.Polyline({
            path: coordinatesArray,
            geodesic: true,
            strokeColor: '#e600e6',
            strokeOpacity: 3.0,
            strokeWeight: 2
        });
        line.setMap($scope.gMap);
    }



    /*      report view page , third page for employee        */
   

    $scope.getReports = function () {
        //getting user reports
        commonFunctions.getReports(userDetails.userName, userDetails.permission).then(function (dataReturn) {
            var reports = dataReturn.data;
            $scope.userReports = [];
            if (reports) {
                $.each(reports, function (index, report) {
                    //parse datetime to date
                    //var parsedDate = new Date(parseInt(report.date.substr(6)));
                    //var newDate = new Date(parsedDate);
                    //report.date = newDate.getDate() + " - " + ( newDate.getMonth() + 1 ) + " - " + newDate.getFullYear();
                    $scope.userReports.push(report);
                });
            }
        });
    };

    $scope.getReportData = function (index) {
        tempDataIndex = index;
        $scope.tempDataForEditWindow = $scope.userReports[index];

        var ownerReportObj = ($scope.employees).find(emp => emp.userName == $scope.tempDataForEditWindow.reportOwner);
        if (angular.isUndefined(ownerReportObj)) {
            ownerReportObj = userDetails;
        }
        $scope.ownerFullName = ownerReportObj.userName + " - " + ownerReportObj.firstName + " " + ownerReportObj.lastName;

        //manager in charge full name
        $scope.managerInChargeFullName = '';
        if ($scope.tempDataForEditWindow.managerInCharge != null) {
            $scope.managerInChargeFullName = $scope.tempDataForEditWindow.managerInCharge.userName + " - " + $scope.tempDataForEditWindow.managerInCharge.firstName + " " + $scope.tempDataForEditWindow.managerInCharge.lastName;
        }

        switch ($scope.tempDataForEditWindow.reportType) {
            case "changing encryptor status":
                document.getElementById('down').href = $scope.userReports[index].reference;
                break;
            case "deliver to employee":
                let newOwner = ($scope.employees).find(emp => emp.userName == $scope.tempDataForEditWindow.enc.ownerID);
                if (angular.isUndefined(newOwner)) {
                    newOwner = userDetails;
                }
                $scope.newOwnerFullName = newOwner.userName + " - " + newOwner.firstName + " " + newOwner.lastName;

            case "changing encryptor location":
                $scope.currentEncData = ($scope.userEncryptors).find(enc => enc.serialNumber == $scope.tempDataForEditWindow.enc.serialNumber);
                break;
            default:
                break;
        }
    }



}]);

/* $scope.getReportData = function (index) {
        tempDataIndex = index;
        $scope.tempDataForEditWindow = $scope.userReports[index];
        var ownerReportObj = ($scope.employees).find(emp => emp.userName == $scope.tempDataForEditWindow.reportOwner);
        $scope.ownerFullName = ownerReportObj.userName + " - " + ownerReportObj.firstName + " " + ownerReportObj.lastName;
        document.getElementById('down').href = $scope.userReports[index].reference;
    }*/
