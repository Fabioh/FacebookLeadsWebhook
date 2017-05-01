function faceBookLogin() {
	FB.getLoginStatus(function (response) {
		FB.login(function (response) {
			statusChangeCallback(response);
		}, { scope: 'public_profile, email, manage_pages' });
	}, true);
}

// This is called with the results from from FB.getLoginStatus().
function statusChangeCallback(response) {
	// The response object is returned with a status field that lets the
	// app know the current login status of the person.
	// Full docs on the response object can be found in the documentation
	// for FB.getLoginStatus().
	if (response.status === 'connected') {
		// Logged into your app and Facebook.
		console.log('conectado com sucesso');
		storeLoginResponse(response.authResponse, function () {
			testAPI();
			havePermissionToUserPages();
		});
	} else {
		// The person is not logged into your app or we are unable to tell.
		$('#status').html("<label class='col-md-8'>por favor entre na aplicação com o Facebook.</label>");
	}
}

// Here we run a very simple test of the Graph API after login is
// successful.  See statusChangeCallback() for when this call is made.
function testAPI() {
	console.log('Bem vindo! Obtendo informações....');
	FB.api('/me', function (response) {
		$('#status').html("<label class='col-md-8'>Obrigado por entrar, " + response.name + "!</label>");
		$('#btnLogin').attr('disabled', 'disabled');
	});
}

function havePermissionToUserPages() {
	FB.api('/me/permissions',
		'GET',
		{ "fields": "permission,status" },
		function (response) {
			permissionFound = false;
			for (var i = 0; i < response.data.length && !permissionFound; i++) {
				if (response.data[i].permission === "manage_pages" && response.data[i].status === "granted") {
					permissionFound = true;
				}
			}
			// the app have the permission to view the pages
			if (permissionFound === true) {
				// load the dropdown with use pages
				loadUserAdminPages();
			} else {
				// request permissions to view pages
				FB.login(function (response) {
					if (response.status === 'connected') {
						console.log('Conectado com sucesso');
						loadUserAdminPages();
					} else {
						console.log('Não foi possivel connectar')
					}
				}, { scope: 'manage_pages' });
			}
		});
}

function loadUserAdminPages() {
	FB.api('/me/accounts',
		function (response) {
			if (response && !response.error) {
				console.log('response me/account', response.data);
				if (response.data.length > 0) {
					if ($("#lblPages").length === 0) {
						$("#pagesContainer").append("<label id='lblPages' for='drpPages' class='col-md-3'>Suas Paginas:</label>");
					}
					$("#drpPages").remove();
					$("#pagesContainer").append("<select id='drpPages' name='pages' class='col-md-5'></select>");
					$("#drpPages").append("<option value=''> -- Selecione -- </option>");
					$.each(response.data, function (index, value) {
						$('#drpPages').append("<option value='" + value.id + "'>" + value.name + "</option>");
						$("#pagesContainer").append("<input type='hidden' id='" + value.id + "_id' value='" + value.access_token + "' />");
					});
					$("#drpPages").bind("change", drpPagesChange);
				} else {
					alert('Você não possui paginas no Facebook');
				}
			}
		});
}

function drpPagesChange() {
	if (hasValue($(this))) {
		//subscribeApp($(this).val(), $("#"+ $(this).val() + "_id").val());
		$("#btnSubscribe").show();
	} else {
		$("#btnSubscribe").hide();
	}
}

function btnSubscribeOnclick() {
	subscribeApp($("#drpPages").val(), $("#" + $("#drpPages").val() + "_id").val());
}

function subscribeApp(pageId, accessToken) {
	FB.api('/' + pageId + '/subscribed_apps',
		'POST',
		{ access_token: accessToken },
		function (response) {
			console.log('Pagina associada com sucesso', response);
			alert("Pagina associada com sucesso");
		});
}

function storeLoginResponse(authResponse, callBack) {
	$.post('/facebook/storeloginresponse', authResponse, function(data) {
		if (data.result.sucess === true) {
			if (hasValue(callBack)) {
				callBack();
			}
		}
	});
}