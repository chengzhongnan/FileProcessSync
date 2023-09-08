# FileProcessSync
文件同步服务器
## 使用方法
1. 使用Git克隆该项目
   ```shell
   git clone https://github.com/chengzhongnan/FileProcessSync.git
   ```
2. 使用VS编译该项目
3. 在目标服务器部署服务器端FileProcessSync
4. 配置目标服务器需要同步的目录，修改App.Config文件，然后启动服务器
5. 修改本地客户端配置，使得本地配置与服务器配置匹配
6. 运行本地FileProcessSyncClient.exe同步
## App.Config配置文件
1. 所有需要修改的配置都在<Server>里面
2. <http>标签下的配置为http服务器监听ip和端口，可以根据需要修改
3. <Directory>标签下配置为同步目录，可以建立多个目录，每个目录都有一个name属性，需要与客户端保持一致，并且服务器端任意两个目录的name不能相同，客户端部分可以相同
4. path的属性为需要同步的路径，可以为绝对路径，也可以是相对路径
5. includeSubDir的属性标记是否同步子目录
6. <dir>下面可以包含多个匹配和排除的正则表达式，并且排除的优先级高于匹配的优先级，一个文件如果能同时匹配match和exclude的规则，则会被排除
7. 目前规则仅仅匹配文件名，不匹配路径
8. 客户端可以配置baseUrl，为调用api的地址和端口，或者域名，可以配置为将不同dir同步到不同的服务器中
9. <Commands>标签下的为服务器进程的关闭和重新启动，可以参考下面的代码，这里的name可以配置多个，以实现一次性关闭多个进程或者启动多个进程
    ```shell
    			<cmd name="start" file="D:\Chaomos\Bin\Server.exe" args=""  workdir="D:\Chaomos\Bin\"></cmd>
			    <cmd name="stop" file="taskkill.exe" args=" /F /IM  Server.exe" workdir="C:\Windows\System32\"></cmd>
    ```
