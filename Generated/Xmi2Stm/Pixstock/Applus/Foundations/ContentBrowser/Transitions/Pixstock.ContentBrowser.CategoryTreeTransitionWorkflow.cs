using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Hyperion.Pf.Workflow;
using Appccelerate.StateMachine;
using Appccelerate.StateMachine.Infrastructure;
using Appccelerate.StateMachine.Machine;
using Appccelerate.StateMachine.Persistence;
using Appccelerate.StateMachine.Reports;
using Hyperion.Pf.Workflow.StateMachine;

using Pixstock.Core;
namespace Pixstock.Applus.Foundations.ContentBrowser.Transitions {
public partial class CategoryTreeTransitionWorkflow : FrameStateMachine<States, Events>, IAsyncPassiveStateMachine {
	public static string Name = "Pixstock.ContentBrowser.CategoryTreeTransitionWorkflow";
public void Setup() {
DefineHierarchyOn(States.ROOT)
.WithHistoryType(HistoryType.None)
.WithInitialSubState(States.Dashboard)
;
DefineHierarchyOn(States.Dashboard)
.WithHistoryType(HistoryType.None)
.WithInitialSubState(States.DashboardBase)
.WithSubState(States.Finder)
;
DefineHierarchyOn(States.Finder)
.WithHistoryType(HistoryType.None)
.WithInitialSubState(States.FinderBase)
.WithSubState(States.Previews)
;
DefineHierarchyOn(States.Previews)
.WithHistoryType(HistoryType.None)
.WithInitialSubState(States.PreviewBase)
.WithSubState(States.Preview)
.WithSubState(States.ContentListPreview)
;
In(States.INIT)
.On(Events.TRNS_TOPSCREEN)
.Goto(States.Dashboard);
In(States.ROOT)
.On(Events.TRNS_EXIT)
.Goto(States.INIT);
In(States.ROOT)
.On(Events.TRNS_DEBUG_BACK)
.Goto(States.ROOT);
In(States.DashboardBase)
.On(Events.TRNS_FinderScreen)
.Goto(States.Finder);
In(States.FinderBase)
.On(Events.TRNS_PreviewPage)
.Goto(States.Preview);
In(States.FinderBase)
.On(Events.TRNS_BACK)
.Goto(States.Dashboard);
In(States.FinderBase)
.On(Events.TRNS_ContentListPreview)
.Goto(States.ContentListPreview);
In(States.Preview)
.On(Events.TRNS_BACK)
.Goto(States.FinderBase);
In(States.ContentListPreview)
.On(Events.TRNS_BACK)
.Goto(States.FinderBase);
In(States.ROOT)
.On(Events.RESPONSE_GETCATEGORY)
.Execute<object>(RESPONSE_GETCATEGORY);
In(States.ROOT)
.On(Events.RESPONSE_GETCATEGORYCONTENT)
.Execute<object>(RESPONSE_GETCATEGORYCONTENT);
In(States.ROOT)
.On(Events.ACT_DEBUGCOMMAND)
.Execute<object>(ACT_DEBUGCOMMAND);
In(States.ROOT)
.On(Events.ACT_REQINVALIDATE_CATEGORYTREE)
.Execute<object>(ACT_REQINVALIDATE_CATEGORYTREE);
In(States.ROOT)
.On(Events.ACT_RESINVALIDATE_CATEGORYTREE)
.Execute<object>(ACT_RESINVALIDATE_CATEGORYTREE);
In(States.ROOT)
.On(Events.ACT_REQINVALIDATE_CONTENTLIST)
.Execute<object>(ACT_REQINVALIDATE_CONTENTLIST);
In(States.ROOT)
.On(Events.ACT_RESINVALIDATE_CONTENTLIST)
.Execute<object>(ACT_RESINVALIDATE_CONTENTLIST);
In(States.ROOT)
.On(Events.ACT_REQINVALIDATE_CATEGORYLIST)
.Execute<object>(ACT_REQINVALIDATE_CATEGORYLIST);
In(States.ROOT)
.On(Events.ACT_UPDATED_CATEGORYLIST)
.Execute<object>(ACT_UPDATED_CATEGORYLIST);
In(States.ROOT)
.On(Events.ACT_REQINVALIDATE_PREVIEW)
.Execute<object>(ACT_REQINVALIDATE_PREVIEW);
In(States.ROOT)
.On(Events.ACT_RESINVALIDATE_CONTENT)
.Execute<object>(ACT_RESINVALIDATE_CONTENT);
In(States.ROOT)
.On(Events.ACT_REQINVALIDATE_LABELTREE)
.Execute<object>(ACT_REQINVALIDATE_LABELTREE);
In(States.ROOT)
.On(Events.ACT_UPDATED_LABELTREE)
.Execute<object>(ACT_UPDATED_LABELTREE);
In(States.Dashboard)
.ExecuteOnEntry(__FTC_Event_Dashboard_Entry);
In(States.Dashboard)
.ExecuteOnExit(__FTC_Event_Dashboard_Exit);
In(States.DashboardBase)
.ExecuteOnEntry(HomePageBase_Entry);
In(States.DashboardBase)
.ExecuteOnExit(HomePageBase_Exit);
In(States.Finder)
.ExecuteOnEntry(ThumbnailListPage_Entry);
In(States.Finder)
.ExecuteOnExit(ThumbnailListPage_Exit);
In(States.FinderBase)
.On(Events.CategorySelectBtnClick)
.Execute<object>(CategorySelectBtnClick);
In(States.FinderBase)
.On(Events.ACT_ContinueCategoryList)
.Execute<object>(ACT_ContinueCategoryList);
In(States.FinderBase)
.On(Events.ACT_UpperCategoryList)
.Execute<object>(ACT_UpperCategoryList);
In(States.Previews)
.On(Events.ACT_STORE_CONTENTPROP)
.Execute<object>(ACT_STORE_CONTENTPROP);
In(States.Preview)
.ExecuteOnEntry(__FTC_Event_Preview_Entry);
In(States.Preview)
.ExecuteOnExit(__FTC_Event_Preview_Exit);
In(States.Preview)
.On(Events.ACT_DISPLAY_PREVIEWCURRENTLIST)
.Execute<object>(ACT_DISPLAY_PREVIEWCURRENTLIST);
In(States.Preview)
.On(Events.RESPONSE_GETCONTENT)
.Execute<object>(RESPONSE_GETCONTENT);
In(States.ContentListPreview)
.ExecuteOnEntry(__FTC_Event_ContentListPreview_Entry);
In(States.ContentListPreview)
.ExecuteOnExit(__FTC_Event_ContentListPreview_Exit);
	Initialize(States.INIT);
}
public virtual async Task RESPONSE_GETCATEGORY(object param) {
	Events.RESPONSE_GETCATEGORY.FireInvokeWorkflowEvent(new WorkflowMessageEventArgs(param));
	await OnRESPONSE_GETCATEGORY(param);
	Events.RESPONSE_GETCATEGORY.FireCallbackWorkflowEvent(new WorkflowMessageEventArgs(param));
}
public virtual async Task RESPONSE_GETCATEGORYCONTENT(object param) {
	Events.RESPONSE_GETCATEGORYCONTENT.FireInvokeWorkflowEvent(new WorkflowMessageEventArgs(param));
	await OnRESPONSE_GETCATEGORYCONTENT(param);
	Events.RESPONSE_GETCATEGORYCONTENT.FireCallbackWorkflowEvent(new WorkflowMessageEventArgs(param));
}
public virtual async Task ACT_DEBUGCOMMAND(object param) {
	Events.ACT_DEBUGCOMMAND.FireInvokeWorkflowEvent(new WorkflowMessageEventArgs(param));
	await OnACT_DEBUGCOMMAND(param);
	Events.ACT_DEBUGCOMMAND.FireCallbackWorkflowEvent(new WorkflowMessageEventArgs(param));
}
public virtual async Task ACT_REQINVALIDATE_CATEGORYTREE(object param) {
	Events.ACT_REQINVALIDATE_CATEGORYTREE.FireInvokeWorkflowEvent(new WorkflowMessageEventArgs(param));
	await OnACT_REQINVALIDATE_CATEGORYTREE(param);
	Events.ACT_REQINVALIDATE_CATEGORYTREE.FireCallbackWorkflowEvent(new WorkflowMessageEventArgs(param));
}
public virtual async Task ACT_RESINVALIDATE_CATEGORYTREE(object param) {
	Events.ACT_RESINVALIDATE_CATEGORYTREE.FireInvokeWorkflowEvent(new WorkflowMessageEventArgs(param));
	await OnACT_RESINVALIDATE_CATEGORYTREE(param);
	Events.ACT_RESINVALIDATE_CATEGORYTREE.FireCallbackWorkflowEvent(new WorkflowMessageEventArgs(param));
}
public virtual async Task ACT_REQINVALIDATE_CONTENTLIST(object param) {
	Events.ACT_REQINVALIDATE_CONTENTLIST.FireInvokeWorkflowEvent(new WorkflowMessageEventArgs(param));
	await OnACT_REQINVALIDATE_CONTENTLIST(param);
	Events.ACT_REQINVALIDATE_CONTENTLIST.FireCallbackWorkflowEvent(new WorkflowMessageEventArgs(param));
}
public virtual async Task ACT_RESINVALIDATE_CONTENTLIST(object param) {
	Events.ACT_RESINVALIDATE_CONTENTLIST.FireInvokeWorkflowEvent(new WorkflowMessageEventArgs(param));
	await OnACT_RESINVALIDATE_CONTENTLIST(param);
	Events.ACT_RESINVALIDATE_CONTENTLIST.FireCallbackWorkflowEvent(new WorkflowMessageEventArgs(param));
}
public virtual async Task ACT_REQINVALIDATE_CATEGORYLIST(object param) {
	Events.ACT_REQINVALIDATE_CATEGORYLIST.FireInvokeWorkflowEvent(new WorkflowMessageEventArgs(param));
	await OnACT_REQINVALIDATE_CATEGORYLIST(param);
	Events.ACT_REQINVALIDATE_CATEGORYLIST.FireCallbackWorkflowEvent(new WorkflowMessageEventArgs(param));
}
public virtual async Task ACT_UPDATED_CATEGORYLIST(object param) {
	Events.ACT_UPDATED_CATEGORYLIST.FireInvokeWorkflowEvent(new WorkflowMessageEventArgs(param));
	await OnACT_UPDATED_CATEGORYLIST(param);
	Events.ACT_UPDATED_CATEGORYLIST.FireCallbackWorkflowEvent(new WorkflowMessageEventArgs(param));
}
public virtual async Task ACT_REQINVALIDATE_PREVIEW(object param) {
	Events.ACT_REQINVALIDATE_PREVIEW.FireInvokeWorkflowEvent(new WorkflowMessageEventArgs(param));
	await OnACT_REQINVALIDATE_PREVIEW(param);
	Events.ACT_REQINVALIDATE_PREVIEW.FireCallbackWorkflowEvent(new WorkflowMessageEventArgs(param));
}
public virtual async Task ACT_RESINVALIDATE_CONTENT(object param) {
	Events.ACT_RESINVALIDATE_CONTENT.FireInvokeWorkflowEvent(new WorkflowMessageEventArgs(param));
	await OnACT_RESINVALIDATE_CONTENT(param);
	Events.ACT_RESINVALIDATE_CONTENT.FireCallbackWorkflowEvent(new WorkflowMessageEventArgs(param));
}
public virtual async Task ACT_REQINVALIDATE_LABELTREE(object param) {
	Events.ACT_REQINVALIDATE_LABELTREE.FireInvokeWorkflowEvent(new WorkflowMessageEventArgs(param));
	await OnACT_REQINVALIDATE_LABELTREE(param);
	Events.ACT_REQINVALIDATE_LABELTREE.FireCallbackWorkflowEvent(new WorkflowMessageEventArgs(param));
}
public virtual async Task ACT_UPDATED_LABELTREE(object param) {
	Events.ACT_UPDATED_LABELTREE.FireInvokeWorkflowEvent(new WorkflowMessageEventArgs(param));
	await OnACT_UPDATED_LABELTREE(param);
	Events.ACT_UPDATED_LABELTREE.FireCallbackWorkflowEvent(new WorkflowMessageEventArgs(param));
}
public virtual async Task __FTC_Event_Dashboard_Entry() {
ICollection<int> ribbonMenuEventId = new List<int>{  };
	ShowFrame("Dashboard",ribbonMenuEventId);
}
public virtual async Task __FTC_Event_Dashboard_Exit() {
ICollection<int> ribbonMenuEventId = new List<int>{  };
	HideFrame("Dashboard", ribbonMenuEventId);
}
public virtual async Task HomePageBase_Entry() {
	await OnHomePageBase_Entry();
}
public virtual async Task HomePageBase_Exit() {
	await OnHomePageBase_Exit();
}
public virtual async Task ThumbnailListPage_Entry() {
	await OnThumbnailListPage_Entry();
ICollection<int> ribbonMenuEventId = new List<int>{  };
	ShowFrame("Finder",ribbonMenuEventId);
}
public virtual async Task ThumbnailListPage_Exit() {
	await OnThumbnailListPage_Exit();
ICollection<int> ribbonMenuEventId = new List<int>{  };
	HideFrame("Finder", ribbonMenuEventId);
}
public virtual async Task CategorySelectBtnClick(object param) {
	Events.CategorySelectBtnClick.FireInvokeWorkflowEvent(new WorkflowMessageEventArgs(param));
	await OnCategorySelectBtnClick(param);
	Events.CategorySelectBtnClick.FireCallbackWorkflowEvent(new WorkflowMessageEventArgs(param));
}
public virtual async Task ACT_ContinueCategoryList(object param) {
	Events.ACT_ContinueCategoryList.FireInvokeWorkflowEvent(new WorkflowMessageEventArgs(param));
	await OnACT_ContinueCategoryList(param);
	Events.ACT_ContinueCategoryList.FireCallbackWorkflowEvent(new WorkflowMessageEventArgs(param));
}
public virtual async Task ACT_UpperCategoryList(object param) {
	Events.ACT_UpperCategoryList.FireInvokeWorkflowEvent(new WorkflowMessageEventArgs(param));
	await OnACT_UpperCategoryList(param);
	Events.ACT_UpperCategoryList.FireCallbackWorkflowEvent(new WorkflowMessageEventArgs(param));
}
public virtual async Task ACT_STORE_CONTENTPROP(object param) {
	Events.ACT_STORE_CONTENTPROP.FireInvokeWorkflowEvent(new WorkflowMessageEventArgs(param));
	await OnACT_STORE_CONTENTPROP(param);
	Events.ACT_STORE_CONTENTPROP.FireCallbackWorkflowEvent(new WorkflowMessageEventArgs(param));
}
public virtual async Task __FTC_Event_Preview_Entry() {
ICollection<int> ribbonMenuEventId = new List<int>{  };
	ShowFrame("Preview",ribbonMenuEventId);
}
public virtual async Task __FTC_Event_Preview_Exit() {
ICollection<int> ribbonMenuEventId = new List<int>{  };
	HideFrame("Preview", ribbonMenuEventId);
}
public virtual async Task ACT_DISPLAY_PREVIEWCURRENTLIST(object param) {
	Events.ACT_DISPLAY_PREVIEWCURRENTLIST.FireInvokeWorkflowEvent(new WorkflowMessageEventArgs(param));
	await OnACT_DISPLAY_PREVIEWCURRENTLIST(param);
	Events.ACT_DISPLAY_PREVIEWCURRENTLIST.FireCallbackWorkflowEvent(new WorkflowMessageEventArgs(param));
}
public virtual async Task RESPONSE_GETCONTENT(object param) {
	Events.RESPONSE_GETCONTENT.FireInvokeWorkflowEvent(new WorkflowMessageEventArgs(param));
	await OnRESPONSE_GETCONTENT(param);
	Events.RESPONSE_GETCONTENT.FireCallbackWorkflowEvent(new WorkflowMessageEventArgs(param));
}
public virtual async Task __FTC_Event_ContentListPreview_Entry() {
ICollection<int> ribbonMenuEventId = new List<int>{  };
	ShowFrame("ContentListPreview",ribbonMenuEventId);
}
public virtual async Task __FTC_Event_ContentListPreview_Exit() {
ICollection<int> ribbonMenuEventId = new List<int>{  };
	HideFrame("ContentListPreview", ribbonMenuEventId);
}
}
}
