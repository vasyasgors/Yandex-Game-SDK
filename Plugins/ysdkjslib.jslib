mergeInto(LibraryManager.library,
{
	YSDKIsMobile: function () {
        var ua = window.navigator.userAgent.toLowerCase();
        var mobilePattern = /android|iphone|ipad|ipod/i;

        return ua.search(mobilePattern) !== -1 || (ua.indexOf("macintosh") !== -1 && "ontouchend" in document);
    },
	
	YSDKSavePlayerProgress: function (data) {
		if (player == null){
			console.log('[Yandex Game SDK JS]: Player not exist!');
			return;
		}
		
		var dataString = UTF8ToString(data);
		var myobj = JSON.parse(dataString);
		player.setData(myobj);
		
		console.log('[Yandex Game SDK JS]: Save player data');
	},
	
	YSDKLoadPlayerProgress: function () {
		if (player == null){
			console.log('[Yandex Game SDK JS]: Player not exist!');
			return;
		}
		
		player.getData().then(_date => {
			const myJSON = JSON.stringify(_date);
			gameInstance.SendMessage('YandexGame', 'OnPlayerProgressLoaded', myJSON);
			console.log('[Yandex Game SDK JS]: Load player data');
		});
		
		console.log('[Yandex Game SDK JS]: After load progress');
	},
	
	 
		
	
	YSDKInit: function () {
	
		YaGames
        .init()
        .then(ysdk => {
			
			window.ysdk = ysdk;
			
			console.log('[Yandex Game SDK JS]: gameInstance = ', gameInstance);
			
			gameInstance.SendMessage('YandexGame', 'OnYandexSDKInit');
			
        });
	},
	
	//----- Payments
	
	YSDKInitPayments : function () {
		ysdk.getPayments()
			.then(_payments => {
				// Методы покупок доступны в payments.
				payments = _payments;
				console.log('[Yandex Game SDK JS]: Payments init OK', payments);
				gameInstance.SendMessage('YandexGame', 'OnPaymentInit');	
		
			}).catch(err => {
				console.log('[Yandex Game SDK JS]: Payments init error', err);
				gameInstance.SendMessage('YandexGame', 'OnPaymentError');	
			});
	},
	
	YSDKLoadPlayerPurchases: function()
	{
		payments.getPurchases()
			.then(purchases => {
				
				 console.log('[Yandex Game SDK JS]:  All Purchse loaded', purchases, "JSON: ", JSON.stringify(purchases));
				 gameInstance.SendMessage('YandexGame', 'OnRecivePlayerPurchases', JSON.stringify(purchases));
				
				 
			}).catch(err => {
				// Ошибка получения списка покупок. Выбрасывает исключение PAYMENT_FAILURE.
				
				console.log('[Yandex Game SDK JS]:  Get Purchases error', err);
				gameInstance.SendMessage('YandexGame', 'OnRecivePlayerPurchasesError');				
			});
	},	
	
	YSDKStartPurchase: function (purchaseId) {
		
		console.log('[Yandex Game SDK JS]: Start purchase', purchaseId);
		var productStr = UTF8ToString(purchaseId);  
	
		payments.purchase({ id: productStr })
		.then(purchase => {
			// Покупка успешно совершена!
			console.log('[Yandex Game SDK JS]: Purchase succses', purchase, "JSON: ", JSON.stringify());
			gameInstance.SendMessage('YandexGame', 'OnPurchaseSuccses',  JSON.stringify(purchase));	
			
		}).catch(err => {
			
			console.log('[Yandex Game SDK JS]:', err);
			gameInstance.SendMessage('YandexGame', 'OnPurchaseError', JSON.stringify(err));
			
			// Покупка не удалась: в Консоли разработчика не добавлен товар с таким id,
			// пользователь не авторизовался, передумал и закрыл окно оплаты,
			// истекло отведенное на покупку время, не хватило денег и т. д.
		});
		
	},
	
	YSDKConsumePurchase: function (purchaseToken) 
	{
		var purchaseTokenSrt = UTF8ToString(purchaseToken);  
		
		payments.consumePurchase(purchaseTokenSrt)
		  .then((result) => {
			   console.log('[Yandex Game SDK JS]: Purchase consumed!');
			   gameInstance.SendMessage('YandexGame', 'OnPurchaseConsumed');
		  })
		  .catch((error) => {
			  console.log('[Yandex Game SDK JS]:', error);
			  gameInstance.SendMessage('YandexGame', 'OnPurchaseConsumedError', JSON.stringify(error));
		  });
	},
	

	YSDKLoadCatalog: function () 
	{
		
		payments.getCatalog()
			.then(products => {
				console.log('[Yandex Game SDK JS]: GameShop loaded: JSON', JSON.stringify(products));
				gameInstance.SendMessage('YandexGame', 'OnCatalogLoaded', JSON.stringify(products));
			});
	},
	

	//----------------
	
	YSDKInitPlayer: function () {
		initPlayer().then(_player => {
			
			if (_player.isAuthorized() === false) {
				
			console.log(' [Yandex Game SDK JS]: Player not auth')
			   gameInstance.SendMessage('YandexGame', 'OnPlayerNotAuth');
			}
			else{
				console.log('[Yandex Game SDK JS]: Player auth')
				gameInstance.SendMessage('YandexGame', 'OnPlayerAuth');
			}
			
		}).catch(err => {
			// Ошибка при инициализации объекта Player.
			console.log('[Yandex Game SDK JS]: Ошибка при инициализации объекта Player.')
			// Временно
			gameInstance.SendMessage('YandexGame', 'OnPlayerNotAuth');
		});
	
	},
	
	
	YSDKGameplayAPIReady: function () {
		if (ysdk !== null && ysdk.features !== undefined && ysdk.features.LoadingAPI !== undefined) {
			ysdk.features.LoadingAPI.ready();
			console.log('[Yandex Game SDK JS]:  LoadingAPI Ready')
		}
		else {
			if (ysdk == null) console.error('Gameplay ready rejected. The SDK is not initialized!');
			else console.error('[Yandex Game SDK JS]:  Gameplay ready undefined!');
		}
	},
	
	YSDKGameplayAPIStart: function () {
		if (ysdk !== null && ysdk.features !== undefined && ysdk.features.GameplayAPI !== undefined) {
			ysdk.features.GameplayAPI.start();
			console.log('[Yandex Game SDK JS]:  GameplayAPI Start')
		}
		else {
			if (ysdk == null) console.error('Gameplay start rejected. The SDK is not initialized!');
			else console.error('[Yandex Game SDK JS]: Gameplay start undefined!');
		}
	},
	
	YSDKGameplayAPIStop: function () {
		if (ysdk !== null && ysdk.features !== undefined && ysdk.features.GameplayAPI !== undefined) {
			ysdk.features.GameplayAPI.stop();
			console.log('[Yandex Game SDK JS]: GameplayAPI Stop')
		}
		else {
			if (ysdk == null) console.error('[Yandex Game SDK JS]: Gameplay stop rejected. The SDK is not initialized!');
			else console.error('[Yandex Game SDK JS]: Gameplay stop undefined!');
		}
	},
	
	YSDKShowStickyBanner : function(){
		ysdk.adv.showBannerAdv();
	},
	
	YSDKHideStickyBanner : function(){
		ysdk.adv.hideBannerAdv();
	},
	
	
	
	YSDKShowFullscreenAd : function(){
		
		console.log('[Yandex Game SDK JS]: FullscreenAd start show');
		ysdk.adv.showFullscreenAdv({
			callbacks: {
				onOpen : function() {
					console.log('[Yandex Game SDK JS]: Video ad open.');
					gameInstance.SendMessage('YandexGame', 'OnFullscreenAdOpen');
				  // Действие после закрытия рекламы.
				},
				
				onClose: function(wasShown) {
					console.log('[Yandex Game SDK JS]: Video ad close.');
					gameInstance.SendMessage('YandexGame', 'OnFullscreenAdClosed');
				  // Действие после закрытия рекламы.
				},
				onError: function(error) {
					console.log('[Yandex Game SDK JS]: Error while open video ad: ', error);
					gameInstance.SendMessage('YandexGame', 'OnFullscreenAdError');
				  // Действие в случае ошибки.
				}
				
			}
		})	
	},
	
	YSDKShowRewardedAd : function(){
		console.log('[Yandex Game SDK JS]: FullscreenAd start show');
		ysdk.adv.showRewardedVideo({
			callbacks: {
				onOpen: () => {
				  console.log('[Yandex Game SDK JS]: Video ad open.');
				  gameInstance.SendMessage('YandexGame', 'OnRewardedAdOpen');
				},
				onRewarded: () => {
				  console.log('[Yandex Game SDK JS]: Rewarded!');
				  gameInstance.SendMessage('YandexGame', 'OnRewardedAdRewarded');
				},
				onClose: () => {
				  console.log('[Yandex Game SDK JS]: Video ad closed.');
				  gameInstance.SendMessage('YandexGame', 'OnRewardedAdClose');
				},
				onError: (error) => {
				  console.log('[Yandex Game SDK JS]: Error while open video ad:', error);
				  gameInstance.SendMessage('YandexGame', 'OnRewardedAdError');
				}
			}
		})
		
	},
	
	
	YSDKGetLanguage : function(){
		
		var lang = ysdk.environment.i18n.lang;
		 
		var bufferSize = lengthBytesUTF8(lang) + 1;
		var buffer = _malloc(bufferSize);
		stringToUTF8(lang, buffer, bufferSize);
		return buffer;
	},
	
	
	YSDKGetServerTime : function(){
		
		return JSON.stringify( ysdk.serverTime());
	},
	
	
});