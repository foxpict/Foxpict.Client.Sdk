using System;
using Hyperion.Pf.Workflow;
using Hyperion.Pf.Workflow.StateMachine;
using Appccelerate.StateMachine;
using Appccelerate.StateMachine.Infrastructure;
using Appccelerate.StateMachine.Machine;
using Appccelerate.StateMachine.Persistence;
using Appccelerate.StateMachine.Reports;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
namespace Pixstock.Core {
public class CLS_INIT : States {}
public class CLS_ROOT : States {}
public class CLS_Dashboard : States {}
public class CLS_DashboardBase : States {}
public class CLS_Finder : States {}
public class CLS_FinderBase : States {}
public class CLS_Previews : States {}
public class CLS_Preview : States {}
public class CLS_ContentListPreview : States {}
public class CLS_PreviewBase : States {}
public class CLS_RESPONSE_GETCATEGORY : Events {}
public class CLS_RESPONSE_GETCATEGORYCONTENT : Events {}
public class CLS_ACT_DEBUGCOMMAND : Events {}
public class CLS_ACT_REQINVALIDATE_CATEGORYTREE : Events {}
public class CLS_ACT_RESINVALIDATE_CATEGORYTREE : Events {}
public class CLS_ACT_REQINVALIDATE_CONTENTLIST : Events {}
public class CLS_ACT_RESINVALIDATE_CONTENTLIST : Events {}
public class CLS_ACT_REQINVALIDATE_CATEGORYLIST : Events {}
public class CLS_ACT_UPDATED_CATEGORYLIST : Events {}
public class CLS_ACT_REQINVALIDATE_PREVIEW : Events {}
public class CLS_ACT_RESINVALIDATE_CONTENT : Events {}
public class CLS_ACT_REQINVALIDATE_LABELTREE : Events {}
public class CLS_ACT_UPDATED_LABELTREE : Events {}
public class CLS_CategorySelectBtnClick : Events {}
public class CLS_ACT_ContinueCategoryList : Events {}
public class CLS_ACT_UpperCategoryList : Events {}
public class CLS_ACT_STORE_CONTENTPROP : Events {}
public class CLS_ACT_DISPLAY_PREVIEWCURRENTLIST : Events {}
public class CLS_RESPONSE_GETCONTENT : Events {}
public class CLS_TRNS_TOPSCREEN : Events {}
public class CLS_TRNS_EXIT : Events {}
public class CLS_TRNS_FinderScreen : Events {}
public class CLS_TRNS_PreviewPage : Events {}
public class CLS_TRNS_BACK : Events {}
public class CLS_TRNS_DEBUG_BACK : Events {}
public class CLS_TRNS_ContentListPreview : Events {}
public class CLSINVALID_INVALID : Events {}
public partial class States : WorkflowStateBase {
	public static CLS_INIT INIT { get; } = new CLS_INIT();
	public static CLS_ROOT ROOT { get; } = new CLS_ROOT();
	public static CLS_Dashboard Dashboard { get; } = new CLS_Dashboard();
	public static CLS_DashboardBase DashboardBase { get; } = new CLS_DashboardBase();
	public static CLS_Finder Finder { get; } = new CLS_Finder();
	public static CLS_FinderBase FinderBase { get; } = new CLS_FinderBase();
	public static CLS_Previews Previews { get; } = new CLS_Previews();
	public static CLS_Preview Preview { get; } = new CLS_Preview();
	public static CLS_ContentListPreview ContentListPreview { get; } = new CLS_ContentListPreview();
	public static CLS_PreviewBase PreviewBase { get; } = new CLS_PreviewBase();
}
public partial class Events : WorkflowEventBase {
        public static Dictionary<string, Events> cacheEventsDict = null;
        public static Events ForName(string name)
        {
            if (cacheEventsDict != null) return cacheEventsDict[name];

            cacheEventsDict = new Dictionary<string, Events>();
            var hogeType = typeof(Events);
            var names = hogeType.GetProperties(BindingFlags.Static | BindingFlags.Public)
                  .Where(x => x.PropertyType.IsSubclassOf(typeof(Events)))
                  .Select(x => new { Name = x.Name, Value = x.GetValue(hogeType, null) as Events }).ToList();
            foreach (var o in names)
            {
                cacheEventsDict.Add(o.Name, o.Value);
            }

            return cacheEventsDict[name];
        }
	public static CLS_RESPONSE_GETCATEGORY RESPONSE_GETCATEGORY { get; } = new CLS_RESPONSE_GETCATEGORY();
	public static CLS_RESPONSE_GETCATEGORYCONTENT RESPONSE_GETCATEGORYCONTENT { get; } = new CLS_RESPONSE_GETCATEGORYCONTENT();
	public static CLS_ACT_DEBUGCOMMAND ACT_DEBUGCOMMAND { get; } = new CLS_ACT_DEBUGCOMMAND();
	public static CLS_ACT_REQINVALIDATE_CATEGORYTREE ACT_REQINVALIDATE_CATEGORYTREE { get; } = new CLS_ACT_REQINVALIDATE_CATEGORYTREE();
	public static CLS_ACT_RESINVALIDATE_CATEGORYTREE ACT_RESINVALIDATE_CATEGORYTREE { get; } = new CLS_ACT_RESINVALIDATE_CATEGORYTREE();
	public static CLS_ACT_REQINVALIDATE_CONTENTLIST ACT_REQINVALIDATE_CONTENTLIST { get; } = new CLS_ACT_REQINVALIDATE_CONTENTLIST();
	public static CLS_ACT_RESINVALIDATE_CONTENTLIST ACT_RESINVALIDATE_CONTENTLIST { get; } = new CLS_ACT_RESINVALIDATE_CONTENTLIST();
	public static CLS_ACT_REQINVALIDATE_CATEGORYLIST ACT_REQINVALIDATE_CATEGORYLIST { get; } = new CLS_ACT_REQINVALIDATE_CATEGORYLIST();
	public static CLS_ACT_UPDATED_CATEGORYLIST ACT_UPDATED_CATEGORYLIST { get; } = new CLS_ACT_UPDATED_CATEGORYLIST();
	public static CLS_ACT_REQINVALIDATE_PREVIEW ACT_REQINVALIDATE_PREVIEW { get; } = new CLS_ACT_REQINVALIDATE_PREVIEW();
	public static CLS_ACT_RESINVALIDATE_CONTENT ACT_RESINVALIDATE_CONTENT { get; } = new CLS_ACT_RESINVALIDATE_CONTENT();
	public static CLS_ACT_REQINVALIDATE_LABELTREE ACT_REQINVALIDATE_LABELTREE { get; } = new CLS_ACT_REQINVALIDATE_LABELTREE();
	public static CLS_ACT_UPDATED_LABELTREE ACT_UPDATED_LABELTREE { get; } = new CLS_ACT_UPDATED_LABELTREE();
	public static CLS_CategorySelectBtnClick CategorySelectBtnClick { get; } = new CLS_CategorySelectBtnClick();
	public static CLS_ACT_ContinueCategoryList ACT_ContinueCategoryList { get; } = new CLS_ACT_ContinueCategoryList();
	public static CLS_ACT_UpperCategoryList ACT_UpperCategoryList { get; } = new CLS_ACT_UpperCategoryList();
	public static CLS_ACT_STORE_CONTENTPROP ACT_STORE_CONTENTPROP { get; } = new CLS_ACT_STORE_CONTENTPROP();
	public static CLS_ACT_DISPLAY_PREVIEWCURRENTLIST ACT_DISPLAY_PREVIEWCURRENTLIST { get; } = new CLS_ACT_DISPLAY_PREVIEWCURRENTLIST();
	public static CLS_RESPONSE_GETCONTENT RESPONSE_GETCONTENT { get; } = new CLS_RESPONSE_GETCONTENT();
	public static CLS_TRNS_TOPSCREEN TRNS_TOPSCREEN { get; } = new CLS_TRNS_TOPSCREEN();
	public static CLS_TRNS_EXIT TRNS_EXIT { get; } = new CLS_TRNS_EXIT();
	public static CLS_TRNS_FinderScreen TRNS_FinderScreen { get; } = new CLS_TRNS_FinderScreen();
	public static CLS_TRNS_PreviewPage TRNS_PreviewPage { get; } = new CLS_TRNS_PreviewPage();
	public static CLS_TRNS_BACK TRNS_BACK { get; } = new CLS_TRNS_BACK();
	public static CLS_TRNS_DEBUG_BACK TRNS_DEBUG_BACK { get; } = new CLS_TRNS_DEBUG_BACK();
	public static CLS_TRNS_ContentListPreview TRNS_ContentListPreview { get; } = new CLS_TRNS_ContentListPreview();
	public static CLSINVALID_INVALID INVALID { get; } = new CLSINVALID_INVALID();
}
}
