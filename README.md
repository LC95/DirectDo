# Direct Do

这是一个自用的定时命令程序，为什么有了crontab，还要另写一个程序呢。  
主要是crontab做事情的时候太默默无闻了，我希望在执行操作的时候搞点动静（声音，提示框）出来。  

我开发这程序仅仅是为了挂机结束后给我一个提醒。
包含了命令行程序DirectDo.Client和服务器程序DirectDo.Server。  
支持Linux，Win10 1904以上，需要您安装.NET 5运行环境。  

没有数据持久化，而且永远也不会有持久化。退出服务器后设定的定时将无效。

## Direct.Client

可以发送定时任务指令的工具，在运行该程序前要启动Direct.Server。  
./DirectDo.Client -h 获取提示  

示例  
./DirectDo.Client + "Let Me See At" "12:37" -s -t 3 -p 1m  
服务器程序会下一个12:37弹出一个会自动消失的系统通知“Let Me See At”，一共会弹出三次(-t 3)，间隔一分钟(-p 1m)，发出声音(-s)。  

## Direct.Server

建议设置开机自启动  
两者之间通过ZeroMq交互，服务器使用5556端口。  
[使用控制台别名的教程](https://blog.csdn.net/weixin_34850743/article/details/100124969)  

## 未来的计划

像crontab一样，执行一段命令/脚本。
没有感觉这会很有用。
