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
	// don't change the name of this variable; referenced from native code
    protected UnityPlayer mUnityPlayer; 
	// The helper object
    IabHelper mHelper;
    // Debug tag, for logging
    static final String TAG = "FirstStudio.sloter";
    // UID
    static final String PREF_UNIQUE_ID = "PREF_UNIQUE_ID";
    // SKU prefix
    static final String SKU_CONSUME_JB_PREFIX = "jb_";
    // (arbitrary) request code for the purchaseflow
    static final int RC_REQUEST = 10001;
    
    // SKUs for our products: the premiumupgrade (non-consumable) and gas (consumable)
    static String SKU_consume = "";
    static String UNIQUE_id = null;
	
    // Setup activity layout
    @Override protected void onCreate (Bundle savedInstanceState)
    {
        requestWindowFeature(Window.FEATURE_NO_TITLE);
        super.onCreate(savedInstanceState);

        getWindow().setFormat(PixelFormat.RGBX_8888); // <--- This makes xperia play happy

        mUnityPlayer = new UnityPlayer(this);
        setContentView(mUnityPlayer);
        mUnityPlayer.requestFocus();
        
        LogToFile.init(this);        
        String base64EncodedPublicKey ="MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAhnlY+4bHOJO/7EPizRKqp3loDGq38WLAip2oa4Wtnx4AIqI6f0bK+Xm8f51EFoRSFETmDAUmVZSofzJy5IlORaQxXeyNa50nZ5JZQ8brdp4tueHem1Xo1Qdd+bm8BiSP6qU5XWSDRdlwagqUc7jr/HmVeTrLUPMTBMUz+yQVhGCS2pa8ZNn0lhl5AZ6/JVZHZx/7lAXqO9jMS7e1kjRQM721YfJjHPW/b+9bzkafPJsxqCATdrMbEsMdZ19eh0HWlNpuiRE6I6FelV2GB4VqsqhbT0pRE+AKkHZnp7NvbJ9vH2EdItqpcs3LLGaBa2Rc2hFvxLOQ6+Q4GaLo8r7pWwIDAQAB";
		// Create the helper, passing it our contextand the public key to verify signatures with
		DebugLog(TAG, "Creating IABhelper.");
		mHelper = new IabHelper(this, base64EncodedPublicKey);
		// enable debug logging (for aproduction application, you should set this to false).
		mHelper.enableDebugLogging(true);
		
		// Start setup. This is asynchronousand the specified listener
		// will be called once setup completes.
		DebugLog(TAG, "Startingsetup.");
		mHelper.startSetup(new IabHelper.OnIabSetupFinishedListener() 
		{
			public void onIabSetupFinished(IabResult result)
			{
				DebugLog(TAG, "Setupfinished.");
				if (!result.isSuccess())
				{
					// Oh noes, there was a problem.
					complain("Problemsetting up in-app billing: " + result);					
					return;
				}
				
				// Have we been disposed of inthe meantime? If so, quit.
				if (mHelper == null) return;
				// IAB is fully set up. Now, let'sget an inventory of stuff we own.
				DebugLog(TAG, "Setupsuccessful. Querying inventory.");
				try
				{
                    mHelper.queryInventoryAsync(mGotInventoryListener);
                } 
                catch (IabAsyncInProgressException e) 
                {
                    complain("Error querying inventory. Another async operation in progress.");
                }
			}
		});
    }
    
    @Override protected void onActivityResult(int requestCode, int resultCode, Intent data) 
    {
	    DebugLog(TAG, "onActivityResult(" + requestCode + "," + resultCode + "," + data);
	    
	    // Pass on the activity result to the helper for handling
	    if (!mHelper.handleActivityResult(requestCode, resultCode, data)) 
	    {
	        // not handled, so handle it ourselves (here's where you'd
	        // perform any handling of activity results not related to in-app
	        // billing...
	        DebugLog(TAG, "handle it ourselves.");	        
	        super.onActivityResult(requestCode, resultCode, data);
	    }
	    else 
	    {
	        DebugLog(TAG, "onActivityResult handled by IABUtil.");
	    }
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
        
        // very important:
        DebugLog(TAG, "Destroying helper.");
        if (mHelper != null) {
            mHelper.disposeWhenFinished();
            mHelper = null;
        }
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
	
	// Listener that's called when we finish querying the items and subscriptions weown
	IabHelper.OnIabPurchaseFinishedListener mPurchaseFinishedListener = new IabHelper.OnIabPurchaseFinishedListener()
	{
		public void onIabPurchaseFinished(IabResult result, Purchase purchase)
		{
			DebugLog(TAG, "Purchase finished: " + result + ", purchase:" + purchase);
			if (result.isFailure()) 
			{
			     complain("Error purchasing: " +result); 
			     //setWaitScreen(false);
			     return;
			} 
			
			if (!verifyDeveloperPayload(purchase)) 
			{
				complain("Error purchasing.Authenticity verification failed."); 
				// setWaitScreen(false); 
				return;
			}
			 
			DebugLog(TAG, "Purchase successful."); 
			if (purchase.getSku().equals(SKU_consume)) 
			{ 
				DebugLog(TAG, "Purchase is SKU_consume.Starting consumption.");
				try
				{
					// consume after pay here
					mHelper.consumeAsync(purchase, mConsumeFinishedListener);
				}
				catch (IabAsyncInProgressException e) 
				{
            		complain("Error consumeAsync in onIabPurchaseFinished." + e.getMessage());
        		}
			}
			else
			{
				complain("Unknown purchase:" + purchase.getSku());
			}
		}
	};
	
	// Listener that's called when we finishquerying the items and subscriptions we own
	IabHelper.QueryInventoryFinishedListener mGotInventoryListener = new IabHelper.QueryInventoryFinishedListener() 
	{
		public void onQueryInventoryFinished(IabResult result, Inventory inventory) 
		{
			DebugLog(TAG, "Query inventoryfinished.");
			// Have we been disposed of in themeantime? If so, quit.
			if (mHelper == null) return;
			// Is it a failure?
			if (result.isFailure()) 
			{
				complain("Failed to queryinventory: " + result);
				//return;
			}
			
			DebugLog(TAG, "Query inventorywas successful.");
					
			// Check for gas delivery -- if weown gas, we should fill up the tank immediately
			// Maybe there are some skus
			String sku_jb1 = SKU_CONSUME_JB_PREFIX + "1";
			Purchase jbPurchase = inventory.getPurchase(sku_jb1);
			if (jbPurchase != null &&verifyDeveloperPayload(jbPurchase)) 
			{
				DebugLog(TAG, "We have sku1.Consuming it.");
				try
				{
					mHelper.consumeAsync(inventory.getPurchase(sku_jb1), mConsumeFinishedListener);
				}
				catch (IabAsyncInProgressException e) 
				{
            		complain("Error consumeAsync in onQueryInventoryFinished." + e.getMessage());
        		}
			}
			else
			{
				DebugLog(TAG, "We dont have sku1.");
			}
			
			DebugLog(TAG, "Initial inventoryquery finished; enabling main UI.");
		}
	};

	// Called when consumption is complete
	IabHelper.OnConsumeFinishedListener mConsumeFinishedListener = new IabHelper.OnConsumeFinishedListener() 
	{
		public void onConsumeFinished(Purchase purchase, IabResult result) 
		{
			DebugLog(TAG, "Consumptionfinished. Purchase: " + purchase + ", result: " + result);
			// if we were disposed of in themeantime, quit.
			if (mHelper == null) return;
			
			// We know this is the"gas" sku because it's the only one we consume,
			// so we don't check which sku wasconsumed. If you have more than one
			// sku, you probably shouldcheck...
			
			if (result.isSuccess()) 
			{
				// successfully consumed, so weapply the effects of the item in our
				// game world's logic, which inour case means filling the gas tank a bit
				DebugLog(TAG, "Consumptionsuccessful. Provisioning.");
				
				// send the comsume result to unity
				UnityConsumeComplete(purchase.getPackageName(),
					purchase.getSku(),
					purchase.getToken(),
					purchase.getOrderId(),
					"");
			}
			else 
			{
				complain("Error whileconsuming: " + result);				
			}
			
			DebugLog(TAG, "End consumptionflow.");
		}
	};
	
	boolean verifyDeveloperPayload(Purchase p)
    {
    	String payload = p.getDeveloperPayload();
    	
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
		Log.e(TAG, "**** Sloter Error: " + message);
		LogToFile.d(TAG, "**** Sloter Error: " + message);
		alert("Error: " + message);
	}

	void alert(String message) 
	{
		AlertDialog.Builder bld = new AlertDialog.Builder(this);
		bld.setMessage(message);
		bld.setNeutralButton("OK", null);
		
		DebugLog(TAG, "Showing alert dialog: " + message);
		bld.create().show();
	}
	
	// static UUID
	protected synchronized static String GetUniqueID(Context context) 
	{
		if (UNIQUE_id == null) 
		{
			SharedPreferences sharedPrefs = context.getSharedPreferences(
				PREF_UNIQUE_ID, Context.MODE_PRIVATE);
			UNIQUE_id = sharedPrefs.getString(PREF_UNIQUE_ID, null);
			if (UNIQUE_id == null) 
			{
				UNIQUE_id = UUID.randomUUID().toString();
				Editor editor = sharedPrefs.edit();
				editor.putString(PREF_UNIQUE_ID, UNIQUE_id);
				editor.commit();
			}
		}
		return UNIQUE_id;
	}
	
	// Debug log
	public void DebugLog(String tag, String info)
	{
		Log.d(tag, info);
		LogToFile.d(tag, info);
	}
	
	//-----------------------interactive with unity
	// send message to unity
	void UnityConsumeComplete(String packageName,
        String sku,
        String token,
        String orderId,
        String others)
	{
		UnityPlayer.UnitySendMessage("Main Camera", "Message", "beginConsumeComplete");
		UnityPlayer.UnitySendMessage("Main Camera", "Message", packageName);
		UnityPlayer.UnitySendMessage("Main Camera", "Message", sku);
		UnityPlayer.UnitySendMessage("Main Camera", "Message", token);
		UnityPlayer.UnitySendMessage("Main Camera", "Message", orderId);
		UnityPlayer.UnitySendMessage("Main Camera", "Message", others);
		UnityPlayer.UnitySendMessage("Main Camera", "Message", "endConsumeComplete");
	}
	
	// Pay for unity
	public void Pay(final String buykey)
	{
		DebugLog(TAG, "Try to pay:" + buykey);
		
		SKU_consume = buykey;		
		String payload = "";
		try
		{
			mHelper.launchPurchaseFlow(this, buykey, RC_REQUEST, mPurchaseFinishedListener, payload);
		}
		catch (IabAsyncInProgressException e) 
		{
            complain("Error launchPurchaseFlow:" + e.getMessage());
        }
	}
	
	// UUID for unity
	public String GetUID()
	{
		return GetUniqueID(getApplicationContext());
	}
}
