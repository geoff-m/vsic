debug:
	msbuild /p:Configuration=Debug Visual\ SICXE.sln

release:
	msbuild /p:Configuration=Release Visual\ SICXE.sln

clean:
	rm -rf bin obj
