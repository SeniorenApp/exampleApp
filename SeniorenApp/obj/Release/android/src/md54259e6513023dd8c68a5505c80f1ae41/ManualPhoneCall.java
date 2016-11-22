package md54259e6513023dd8c68a5505c80f1ae41;


public class ManualPhoneCall
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_EnterChar:(Landroid/view/View;)V:__export__\n" +
			"n_Call:(Landroid/view/View;)V:__export__\n" +
			"";
		mono.android.Runtime.register ("SeniorenApp.ManualPhoneCall, SeniorenApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ManualPhoneCall.class, __md_methods);
	}


	public ManualPhoneCall () throws java.lang.Throwable
	{
		super ();
		if (getClass () == ManualPhoneCall.class)
			mono.android.TypeManager.Activate ("SeniorenApp.ManualPhoneCall, SeniorenApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);


	public void EnterChar (android.view.View p0)
	{
		n_EnterChar (p0);
	}

	private native void n_EnterChar (android.view.View p0);


	public void Call (android.view.View p0)
	{
		n_Call (p0);
	}

	private native void n_Call (android.view.View p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
