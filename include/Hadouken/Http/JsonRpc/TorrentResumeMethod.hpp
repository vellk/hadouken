#ifndef HADOUKEN_HTTP_JSONRPC_TORRENTRESUMEMETHOD_HPP
#define HADOUKEN_HTTP_JSONRPC_TORRENTRESUMEMETHOD_HPP

#include <Hadouken/Http/JsonRpc/RpcMethod.hpp>
#include <Poco/JSON/Array.h>

namespace Hadouken
{
    namespace Http
    {
        namespace JsonRpc
        {
            class TorrentResumeMethod : public RpcMethod
            {
            public:
                Poco::Dynamic::Var::Ptr execute(const Poco::JSON::Array::Ptr& params);
            };
        }
    }
}

#endif
