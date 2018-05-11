package com.FirstStudio.Sloter;

import com.unity3d.player.*;
import android.app.Activity;
import android.content.Intent;
import android.content.res.Configuration;
import android.graphics.PixelFormat;
import android.os.Bundle;
import android.view.KeyEvent;
import android.view.MotionEvent;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;

import android.os.Bundle;
import android.util.Log;
import android.app.Activity;
import android.app.AlertDialog;
import android.content.Intent;
import android.content.SharedPreferences;
import android.util.Log;
import android.widget.ImageView;
import android.widget.Toast;

import com.FirstStudio.Slot.util.IabHelper;
import com.FirstStudio.Slot.util.IabResult;
import com.FirstStudio.Slot.util.Inventory;
import com.FirstStudio.Slot.util.Purchase;
import com.FirstStudio.Slot.util.IabHelper.IabAsyncInProgressException;

import java.util.UUID;
import android.content.Context;
import android.content.SharedPreferences.Editor;

public class UnityPlayerActivity extends Activity
{
    protected UnityPlayer mUnityPlayer; // don't change the name of this variable; referenced from native code

	//The helper object
    IabHelper mHelper;
    // Debug tag, for logging
    static final String TAG ="first.studio.slot";
	// Does the user have the premium upgrade?
	boolean mIsPremium = false;
    // Does the user have an activesubscription to the infinite gas plan?
    boolean mSubscribedToInfiniteGas = false;
    // SKUs for our products: the premiumupgrade (non-consumable) and gas (consumable)
    static String SKU_consume ="";
    static String SKU_noconsume ="";
    //static final String SKU_GAS="";
    //SKU for our subscription (infinite gas)
    //static final String SKU_INFINITE_GAS ="infinite_gas";
    // (arbitrary) request code for the purchaseflow
    static final int RC_REQUEST = 10001;
    // Setup activity layout
    @Override protected void onCreate (Bundle savedInstanceState)
    {
        requestWindowFeature(Window.FEATURE_NO_TITLE);
        super.onCreate(savedInstanceState);

        getWindow().setFormat(PixelFormat.RGBX_8888); // <--- This makes xperia play happy

        mUnityPlayer = new UnityPlayer(this);
        setContentView(mUnityPlayer);
        mUnityPlayer.requestFocus();
        
        String base64EncodedPublicKey ="MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAhnlY+4bHOJO/7EPizRKqp3loDGq38WLAip2oa4Wtnx4AIqI6f0bK+Xm8f51EFoRSFETmDAUmVZSofzJy5IlORaQxXeyNa50nZ5JZQ8brdp4tueHem1Xo1Qdd+bm8BiSP6qU5XWSDRdlwagqUc7jr/HmVeTrLUPMTBMUz+yQVhGCS2pa8ZNn0lhl5AZ6/JVZHZx/7lAXqO9jMS7e1kjRQM721YfJjHPW/b+9bzkafPJsxqCATdrMbEsMdZ19eh0HWlNpuiRE6I6FelV2GB4VqsqhbT0pRE+AKkHZnp7NvbJ9vH2EdItqpcs3LLGaBa2Rc2hFvxLOQ6+Q4GaLo8r7pWwIDAQAB";
		// Create the helper, passing it our contextand the public key to verify signatures with
		Log.d(TAG, "Creating IABhelper.");
		mHelper = new IabHelper(this, base64EncodedPublicKey);
		// enable debug logging (for aproduction application, you should set this to false).
		mHelper.enableDebugLogging(true);
		
		// Start setup. This is asynchronousand the specified listener
		// will be called once setup completes.
		Log.d(TAG, "Startingsetup.");
		//complain("onCreate.");
		mHelper.startSetup(new IabHelper.OnIabSetupFinishedListener() 
		{
			public void onIabSetupFinished(IabResult result){
				Log.d(TAG, "Setupfinished.");
				if (!result.isSuccess()){
					// Oh noes, there was a problem.
					complain("Problemsetting up in-app billing: " + result);
					UnityLog("Problemsetting up in-app billing: " + result);
					return;
				}
				
				// Have we been disposed of inthe meantime? If so, quit.
				if (mHelper == null) return;
				// IAB is fully set up. Now, let'sget an inventory of stuff we own.
				Log.d(TAG, "Setupsuccessful. Querying inventory.");
				//complain("Setupsuccessful. Querying inventory.");
				try {
                    mHelper.queryInventoryAsync(mGotInventoryListener);
                } catch (IabAsyncInProgressException e) {
                    complain("Error querying inventory. Another async operation in progress.");
                    UnityLog("Error querying inventory. Another async operation in progress.");
                }
			}
		});
    } 
    
    private static String uniqueID = null;
	private static final String PREF_UNIQUE_ID = "PREF_UNIQUE_ID";
	public synchronized static String GetUniqueID(Context context) {
		if (uniqueID == null) {
			SharedPreferences sharedPrefs = context.getSharedPreferences(
				PREF_UNIQUE_ID, Context.MODE_PRIVATE);
			uniqueID = sharedPrefs.getString(PREF_UNIQUE_ID, null);
			if (uniqueID == null) {
				uniqueID = UUID.randomUUID().toString();
				Editor editor = sharedPrefs.edit();
				editor.putString(PREF_UNIQUE_ID, uniqueID);
				editor.commit();
			}
		}
		return uniqueID;
	}
	public String GetUID()
	{
		return GetUniqueID(getApplicationContext());
	}
	public void Pay(final String buykey)
	{		
		/* TODO: for security, generate your payloadhere for verification. See the comments on
		*        verifyDeveloperPayload() for more info.Since this is a SAMPLE, we just use
		*        an empty string, but on a productionapp you should carefully generate this. */
		if(buykey.contains("jb_"))
		{
			UnityLog("Pay SKU_consume.");
			SKU_consume = buykey;
		}

		//if(buykey.contains("lb_"))
		//{
		//	SKU_noconsume = buykey;
		//}
		
		runOnUiThread(new Runnable()
		{
			public void run()
			{
				Toast.makeText(getApplicationContext(),buykey,Toast.LENGTH_SHORT).show();
				UnityLog("Runable");
				SendToUnityMessage(buykey);
			}
		});
		
		String payload = "";
		try{
			//mHelper.flagEndAsync(); // added by wdz 2018-05-10
			mHelper.launchPurchaseFlow(this,buykey, RC_REQUEST,mPurchaseFinishedListener,payload);
		}catch (IabAsyncInProgressException e) {
            complain("Error launchPurchaseFlow.");
            UnityLog("Error launchPurchaseFlow." + e.getMessage());
        }
	}
	 @Override
	protected void onActivityResult(int requestCode, int resultCode, Intent data) {
	    Log.d(TAG, "onActivityResult(" + requestCode + "," + resultCode + "," + data);
		//try{
		    // Pass on the activity result to the helper for handling
		    if (!mHelper.handleActivityResult(requestCode, resultCode, data)) {
		        // not handled, so handle it ourselves (here's where you'd
		        // perform any handling of activity results not related to in-app
		        // billing...
		        UnityLog("handle it ourselves.");
		        
		        super.onActivityResult(requestCode, resultCode, data);
		    }
		    else {
		        Log.d(TAG, "onActivityResult handled by IABUtil.");
		        UnityLog("onActivityResult handled by IABUtil.");
		    }
//}
	//	catch (IabAsyncInProgressException e) {
           // complain("Error onActivityResult.");
           // UnityLog("Error onActivityResult." + e.getMessage());
//}
	}
	//Listener that's called when we finish querying the items and subscriptions weown
	IabHelper.OnIabPurchaseFinishedListener mPurchaseFinishedListener = new IabHelper.OnIabPurchaseFinishedListener()
	{
		public void onIabPurchaseFinished(IabResult result, Purchase purchase)
		{
//			complain("onIabPurchaseFinished.");
			UnityLog("onIabPurchaseFinished.");
			Log.d(TAG, "Purchase finished: " + result + ", purchase:" + purchase);
			if (result.isFailure()) 
			{
			     complain("Error purchasing: " +result); 
			     UnityLog("Error purchasing: " +result); 
			     //setWaitScreen(false);
			     return;
			} 
			
			if (!verifyDeveloperPayload(purchase)) 
			{
				complain("Error purchasing.Authenticity verification failed."); 
				UnityLog("Error purchasing.Authenticity verification failed.");
				// setWaitScreen(false); 
				return;
			}
			 
			Log.d(TAG, "Purchase successful."); 
			if (purchase.getSku().equals(SKU_consume)) 
			{ 
				complain("consumeAsync SKU_consume.");
				UnityLog("consumeAsync SKU_consume.");
				Log.d(TAG, "Purchase is gas.Starting gas consumption.");
				try{
					mHelper.consumeAsync(purchase,mConsumeFinishedListener); 
				}catch (IabAsyncInProgressException e) {
            		complain("Error consumeAsync in onIabPurchaseFinished.");
            		UnityLog("Error consumeAsync in onIabPurchaseFinished.");
        		}
			}
			else if (purchase.getSku().equals(SKU_noconsume)) 
			{
				Log.d(TAG, "Purchase ispremium upgrade. Congratulating user.");
				alert("Thank you forupgrading to premium!");
				mIsPremium = true;
			}
			else
			{
				UnityLog("unknown purchase.");
			}
		}
	};
	
	// Listener that's called when we finishquerying the items and subscriptions we own
	IabHelper.QueryInventoryFinishedListener mGotInventoryListener = new IabHelper.QueryInventoryFinishedListener() 
	{
		public void onQueryInventoryFinished(IabResult result, Inventory inventory) 
		{
			complain("onQueryInventoryFinished.");
			UnityLog("onQueryInventoryFinished.");
			Log.d(TAG, "Query inventoryfinished.");
			// Have we been disposed of in themeantime? If so, quit.
			if (mHelper == null) return;
			// Is it a failure?
			if (result.isFailure()) 
			{
				complain("Failed to queryinventory: " + result);
				UnityLog("Failed to queryinventory: " + result);
				//return;
			}
			
			String msg = "";
			String sku_jb1 = "jb_1";
			if (inventory.hasPurchase(sku_jb1)) {  
				try{
					msg += "hasPurchase";
					complain("mHelper.consumeAsync in onQueryInventoryFinished1.");
					UnityLog("mHelper.consumeAsync in onQueryInventoryFinished1.");
					mHelper.consumeAsync(inventory.getPurchase(sku_jb1), mConsumeFinishedListener);
				}catch (IabAsyncInProgressException e) {
            		complain("Error consumeAsync1.");
            		UnityLog("mHelper.consumeAsync in onQueryInventoryFinished1111.");            		
				//mHelper.consumeAsync(inventory.getPurchase(SKU_consume), null);
				//UnityLog("mHelper.consumeAsync in onQueryInventoryFinished1.");
				}
			}
			else
			{
				complain("don have SKU_consume.");
			}
			Log.d(TAG, "Query inventorywas successful.");
			/*
			* Check for items we own. Noticethat for each purchase, we check
			* the developer payload to see ifit's correct! See
			* verifyDeveloperPayload().
			*/
			
			// Do we have the premium upgrade?
			Purchase premiumPurchase =inventory.getPurchase(SKU_noconsume);
			mIsPremium = (premiumPurchase !=null && verifyDeveloperPayload(premiumPurchase));
			Log.d(TAG, "User is " +(mIsPremium ? "PREMIUM" : "NOT PREMIUM"));
			
			msg += "PREMIUM";
			if (mIsPremium == false)
				msg += "NOT PREMIUM";
			
			// Check for gas delivery -- if weown gas, we should fill up the tank immediately
			Purchase gasPurchase = inventory.getPurchase(SKU_consume);
			if (gasPurchase != null &&verifyDeveloperPayload(gasPurchase)) 
			{
				Log.d(TAG, "We have gas.Consuming it.");
				try{
					complain("mHelper.consumeAsync in onQueryInventoryFinished2.");
					UnityLog("mHelper.consumeAsync in onQueryInventoryFinished2.");
					mHelper.consumeAsync(inventory.getPurchase(SKU_consume),mConsumeFinishedListener);
				}catch (IabAsyncInProgressException e) {
            		complain("Error consumeAsync2.");
            		UnityLog("Error consumeAsync2.");
        		}
        		msg += ",consumeAsync";
				//return;
			}
			else
				complain("!gasPurchase != null &&verifyDeveloperPayload(gasPurchase).");
			
			alert(msg);
			Log.d(TAG, "Initial inventoryquery finished; enabling main UI.");
		}
	};

	//Called when consumption is complete
	IabHelper.OnConsumeFinishedListener mConsumeFinishedListener = new IabHelper.OnConsumeFinishedListener() 
	{
		public void onConsumeFinished(Purchase purchase, IabResult result) 
		{
			//complain("onConsumeFinished.");
			UnityLog("onConsumeFinished1.");
			Log.d(TAG, "Consumptionfinished. Purchase: " + purchase + ", result: " + result);
			// if we were disposed of in themeantime, quit.
			if (mHelper == null) return;
			
			// We know this is the"gas" sku because it's the only one we consume,
			// so we don't check which sku wasconsumed. If you have more than one
			// sku, you probably shouldcheck...
			
			if (result.isSuccess()) 
			{
				// successfully consumed, so weapply the effects of the item in our
				// game world's logic, which inour case means filling the gas tank a bit
				Log.d(TAG, "Consumptionsuccessful. Provisioning.");
				complain("Consumptionsuccessful. Provisioning.");
				UnityLog("Consumptionsuccessful. Provisioning..");
			}
			else 
			{
				complain("Error whileconsuming: " + result);
				UnityLog("Error whileconsuming: " + result);				
			}
			
			Log.d(TAG, "End consumptionflow.");
		}
	};
	
	boolean verifyDeveloperPayload(Purchase p)
    {
    	String payload =p.getDeveloperPayload();
    	
	    /*
         * TODO: verify that the developerpayload of the purchase is correct. It will be
         * the same one that you sent wheninitiating the purchase.
         *
         * WARNING: Locally generating a randomstring when starting a purchase and
         * verifying it here might seem like agood approach, but this will fail in the
         * case where the user purchases anitem on one device and then uses your app on
         * a different device, because on theother device you will not have access to the
         * random string you originallygenerated.
         *
         * So a good developer payload hasthese characteristics:
         *
         * 1. If two different users purchasean item, the payload is different between them,
         *   so that one user's purchase can't be replayed to another user.
         *
         * 2. The payload must be such that youcan verify it even when the app wasn't the
         *   one who initiated the purchase flow (so that items purchased by the useron
         *   one device work on other devices owned by the user).
         *
         * Using your own server to store andverify developer payloads across app
         * installations is recommended.
         */
        return true;
    }
    
	public void complain(String message) 
	{
		Log.e(TAG, "**** TrivialDrive Error: " + message);
		alert("Error: " + message);
	}

	void alert(String message) 
	{
		AlertDialog.Builder bld = new AlertDialog.Builder(this);
		bld.setMessage(message);
		bld.setNeutralButton("OK", null);
		
		Log.d(TAG, "Showing alert dialog: " + message);
		bld.create().show();
	}

	// send message to unity
	void SendToUnityMessage(String Sendmessage)
	{
		UnityPlayer.UnitySendMessage("Main Camera","Messgae",Sendmessage);
	}
	
	void UnityLog(String str)
	{
		UnityPlayer.UnitySendMessage("Main Camera","Log", str);
	}
	
    @Override protected void onNewIntent(Intent intent)
    {
    	alert("onNewIntent");
        // To support deep linking, we need to make sure that the client can get access to
        // the last sent intent. The clients access this through a JNI api that allows them
        // to get the intent set on launch. To update that after launch we have to manually
        // replace the intent with the one caught here.
        setIntent(intent);
    }

    // Quit Unity
    @Override protected void onDestroy ()
    {
        mUnityPlayer.quit();
        super.onDestroy();
        
        if (mHelper != null) mHelper.dispose();
        mHelper = null;
    }

    // Pause Unity
    @Override protected void onPause()
    {
        super.onPause();
        mUnityPlayer.pause();
    }

    // Resume Unity
    @Override protected void onResume()
    {
        super.onResume();
        mUnityPlayer.resume();
    }

    // Low Memory Unity
    @Override public void onLowMemory()
    {
        super.onLowMemory();
        mUnityPlayer.lowMemory();
    }

    // Trim Memory Unity
    @Override public void onTrimMemory(int level)
    {
        super.onTrimMemory(level);
        if (level == TRIM_MEMORY_RUNNING_CRITICAL)
        {
            mUnityPlayer.lowMemory();
        }
    }

    // This ensures the layout will be correct.
    @Override public void onConfigurationChanged(Configuration newConfig)
    {
        super.onConfigurationChanged(newConfig);
        mUnityPlayer.configurationChanged(newConfig);
    }

    // Notify Unity of the focus change.
    @Override public void onWindowFocusChanged(boolean hasFocus)
    {
        super.onWindowFocusChanged(hasFocus);
        mUnityPlayer.windowFocusChanged(hasFocus);
    }

    // For some reason the multiple keyevent type is not supported by the ndk.
    // Force event injection by overriding dispatchKeyEvent().
    @Override public boolean dispatchKeyEvent(KeyEvent event)
    {
        if (event.getAction() == KeyEvent.ACTION_MULTIPLE)
            return mUnityPlayer.injectEvent(event);
        return super.dispatchKeyEvent(event);
    }

    // Pass any events not handled by (unfocused) views straight to UnityPlayer
    @Override public boolean onKeyUp(int keyCode, KeyEvent event)     { return mUnityPlayer.injectEvent(event); }
    @Override public boolean onKeyDown(int keyCode, KeyEvent event)   { return mUnityPlayer.injectEvent(event); }
    @Override public boolean onTouchEvent(MotionEvent event)          { return mUnityPlayer.injectEvent(event); }
    /*API12*/ public boolean onGenericMotionEvent(MotionEvent event)  { return mUnityPlayer.injectEvent(event); }
}
