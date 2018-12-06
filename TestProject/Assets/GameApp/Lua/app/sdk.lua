local Sdk = class("sdk")

function Sdk:callSdk(num, callFunc)
	SdkManager:CallSdk(num, callFunc)
end

return Sdk