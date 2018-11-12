local LoadingWindow = class("LoadingWindow", import("cframework.mvc.BaseWindow"))


function LoadingWindow:Start()
	
	
	log("LoadingWindow:Start***************"..tostring(self))
end

function LoadingWindow:OnProgress(progress)
	
	self.uiRoot.Sli_loading:GetComponent("Slider").value = tonumber(progress/100);
	self.uiRoot.Txt_loading:GetComponent("Text").text = progress

end



return LoadingWindow
