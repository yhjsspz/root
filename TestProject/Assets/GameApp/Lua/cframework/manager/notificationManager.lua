local NotificationManager = class("NotificationManager")

function NotificationManager:ctor()
    self.observerMap = {}
end

function NotificationManager:registerObserver(id, target, fun)

    local observers = self.observerMap[id]

    if observers == nil then

        self.observerMap[id] = {{target,fun}}

    else

        table.insert(observers,{target,fun})

    end

end

function NotificationManager:removeObserver(id, target, fun)

    local observers = self.observerMap[id]

    if observers == nil then

       return

    end

    for i, v in pairs(observers) do

        if v[1] == target and v[2] == fun then

            table.remove(observers, i)

            break

        end

    end

    if #observers == 0 then

        self.observerMap[id] = nil

    end

end

function NotificationManager:sendNotification(id, sid, data)

    self:notifyObservers(id, sid, data)

end

function NotificationManager:notifyObservers(id, sid, data)

    local observers = self.observerMap[id]

    if observers == nil then

        return
    end

    for i = #observers, 1, -1 do

        local observer = observers[i]
        observer[2](observer[1], id, sid, data)

    end

end

return NotificationManager