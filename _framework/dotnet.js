//! Licensed to the .NET Foundation under one or more agreements.
//! The .NET Foundation licenses this file to you under the MIT license.

var e=!1;const t=async()=>WebAssembly.validate(new Uint8Array([0,97,115,109,1,0,0,0,1,4,1,96,0,0,3,2,1,0,10,8,1,6,0,6,64,25,11,11])),o=async()=>WebAssembly.validate(new Uint8Array([0,97,115,109,1,0,0,0,1,5,1,96,0,1,123,3,2,1,0,10,15,1,13,0,65,1,253,15,65,2,253,15,253,128,2,11])),n=async()=>WebAssembly.validate(new Uint8Array([0,97,115,109,1,0,0,0,1,5,1,96,0,1,123,3,2,1,0,10,10,1,8,0,65,0,253,15,253,98,11])),r=Symbol.for("wasm promise_control");function i(e,t){let o=null;const n=new Promise((function(n,r){o={isDone:!1,promise:null,resolve:t=>{o.isDone||(o.isDone=!0,n(t),e&&e())},reject:e=>{o.isDone||(o.isDone=!0,r(e),t&&t())}}}));o.promise=n;const i=n;return i[r]=o,{promise:i,promise_control:o}}function s(e){return e[r]}function a(e){e&&function(e){return void 0!==e[r]}(e)||Be(!1,"Promise is not controllable")}const l="__mono_message__",c=["debug","log","trace","warn","info","error"],d="MONO_WASM: ";let u,f,m,g,p,h;function w(e){g=e}function b(e){if(Pe.diagnosticTracing){const t="function"==typeof e?e():e;console.debug(d+t)}}function y(e,...t){console.info(d+e,...t)}function v(e,...t){console.info(e,...t)}function E(e,...t){console.warn(d+e,...t)}function _(e,...t){if(t&&t.length>0&&t[0]&&"object"==typeof t[0]){if(t[0].silent)return;if(t[0].toString)return void console.error(d+e,t[0].toString())}console.error(d+e,...t)}function x(e,t,o){return function(...n){try{let r=n[0];if(void 0===r)r="undefined";else if(null===r)r="null";else if("function"==typeof r)r=r.toString();else if("string"!=typeof r)try{r=JSON.stringify(r)}catch(e){r=r.toString()}t(o?JSON.stringify({method:e,payload:r,arguments:n.slice(1)}):[e+r,...n.slice(1)])}catch(e){m.error(`proxyConsole failed: ${e}`)}}}function j(e,t,o){f=t,g=e,m={...t};const n=`${o}/console`.replace("https://","wss://").replace("http://","ws://");u=new WebSocket(n),u.addEventListener("error",A),u.addEventListener("close",S),function(){for(const e of c)f[e]=x(`console.${e}`,T,!0)}()}function R(e){let t=30;const o=()=>{u?0==u.bufferedAmount||0==t?(e&&v(e),function(){for(const e of c)f[e]=x(`console.${e}`,m.log,!1)}(),u.removeEventListener("error",A),u.removeEventListener("close",S),u.close(1e3,e),u=void 0):(t--,globalThis.setTimeout(o,100)):e&&m&&m.log(e)};o()}function T(e){u&&u.readyState===WebSocket.OPEN?u.send(e):m.log(e)}function A(e){m.error(`[${g}] proxy console websocket error: ${e}`,e)}function S(e){m.debug(`[${g}] proxy console websocket closed: ${e}`,e)}function D(){Pe.preferredIcuAsset=O(Pe.config);let e="invariant"==Pe.config.globalizationMode;if(!e)if(Pe.preferredIcuAsset)Pe.diagnosticTracing&&b("ICU data archive(s) available, disabling invariant mode");else{if("custom"===Pe.config.globalizationMode||"all"===Pe.config.globalizationMode||"sharded"===Pe.config.globalizationMode){const e="invariant globalization mode is inactive and no ICU data archives are available";throw _(`ERROR: ${e}`),new Error(e)}Pe.diagnosticTracing&&b("ICU data archive(s) not available, using invariant globalization mode"),e=!0,Pe.preferredIcuAsset=null}const t="DOTNET_SYSTEM_GLOBALIZATION_INVARIANT",o=Pe.config.environmentVariables;if(void 0===o[t]&&e&&(o[t]="1"),void 0===o.TZ)try{const e=Intl.DateTimeFormat().resolvedOptions().timeZone||null;e&&(o.TZ=e)}catch(e){y("failed to detect timezone, will fallback to UTC")}}function O(e){var t;if((null===(t=e.resources)||void 0===t?void 0:t.icu)&&"invariant"!=e.globalizationMode){const t=e.applicationCulture||(ke?globalThis.navigator&&globalThis.navigator.languages&&globalThis.navigator.languages[0]:Intl.DateTimeFormat().resolvedOptions().locale),o=e.resources.icu;let n=null;if("custom"===e.globalizationMode){if(o.length>=1)return o[0].name}else t&&"all"!==e.globalizationMode?"sharded"===e.globalizationMode&&(n=function(e){const t=e.split("-")[0];return"en"===t||["fr","fr-FR","it","it-IT","de","de-DE","es","es-ES"].includes(e)?"icudt_EFIGS.dat":["zh","ko","ja"].includes(t)?"icudt_CJK.dat":"icudt_no_CJK.dat"}(t)):n="icudt.dat";if(n)for(let e=0;e<o.length;e++){const t=o[e];if(t.virtualPath===n)return t.name}}return e.globalizationMode="invariant",null}(new Date).valueOf();const C=class{constructor(e){this.url=e}toString(){return this.url}};async function k(e,t){try{const o="function"==typeof globalThis.fetch;if(Se){const n=e.startsWith("file://");if(!n&&o)return globalThis.fetch(e,t||{credentials:"same-origin"});p||(h=Ne.require("url"),p=Ne.require("fs")),n&&(e=h.fileURLToPath(e));const r=await p.promises.readFile(e);return{ok:!0,headers:{length:0,get:()=>null},url:e,arrayBuffer:()=>r,json:()=>JSON.parse(r),text:()=>{throw new Error("NotImplementedException")}}}if(o)return globalThis.fetch(e,t||{credentials:"same-origin"});if("function"==typeof read)return{ok:!0,url:e,headers:{length:0,get:()=>null},arrayBuffer:()=>new Uint8Array(read(e,"binary")),json:()=>JSON.parse(read(e,"utf8")),text:()=>read(e,"utf8")}}catch(t){return{ok:!1,url:e,status:500,headers:{length:0,get:()=>null},statusText:"ERR28: "+t,arrayBuffer:()=>{throw t},json:()=>{throw t},text:()=>{throw t}}}throw new Error("No fetch implementation available")}function I(e){return"string"!=typeof e&&Be(!1,"url must be a string"),!M(e)&&0!==e.indexOf("./")&&0!==e.indexOf("../")&&globalThis.URL&&globalThis.document&&globalThis.document.baseURI&&(e=new URL(e,globalThis.document.baseURI).toString()),e}const U=/^[a-zA-Z][a-zA-Z\d+\-.]*?:\/\//,P=/[a-zA-Z]:[\\/]/;function M(e){return Se||Ie?e.startsWith("/")||e.startsWith("\\")||-1!==e.indexOf("///")||P.test(e):U.test(e)}let L,N=0;const $=[],z=[],W=new Map,F={"js-module-threads":!0,"js-module-runtime":!0,"js-module-dotnet":!0,"js-module-native":!0,"js-module-diagnostics":!0},B={...F,"js-module-library-initializer":!0},V={...F,dotnetwasm:!0,heap:!0,manifest:!0},q={...B,manifest:!0},H={...B,dotnetwasm:!0},J={dotnetwasm:!0,symbols:!0},Z={...B,dotnetwasm:!0,symbols:!0},Q={symbols:!0};function G(e){return!("icu"==e.behavior&&e.name!=Pe.preferredIcuAsset)}function K(e,t,o){null!=t||(t=[]),Be(1==t.length,`Expect to have one ${o} asset in resources`);const n=t[0];return n.behavior=o,X(n),e.push(n),n}function X(e){V[e.behavior]&&W.set(e.behavior,e)}function Y(e){Be(V[e],`Unknown single asset behavior ${e}`);const t=W.get(e);if(t&&!t.resolvedUrl)if(t.resolvedUrl=Pe.locateFile(t.name),F[t.behavior]){const e=ge(t);e?("string"!=typeof e&&Be(!1,"loadBootResource response for 'dotnetjs' type should be a URL string"),t.resolvedUrl=e):t.resolvedUrl=ce(t.resolvedUrl,t.behavior)}else if("dotnetwasm"!==t.behavior)throw new Error(`Unknown single asset behavior ${e}`);return t}function ee(e){const t=Y(e);return Be(t,`Single asset for ${e} not found`),t}let te=!1;async function oe(){if(!te){te=!0,Pe.diagnosticTracing&&b("mono_download_assets");try{const e=[],t=[],o=(e,t)=>{!Z[e.behavior]&&G(e)&&Pe.expected_instantiated_assets_count++,!H[e.behavior]&&G(e)&&(Pe.expected_downloaded_assets_count++,t.push(se(e)))};for(const t of $)o(t,e);for(const e of z)o(e,t);Pe.allDownloadsQueued.promise_control.resolve(),Promise.all([...e,...t]).then((()=>{Pe.allDownloadsFinished.promise_control.resolve()})).catch((e=>{throw Pe.err("Error in mono_download_assets: "+e),Xe(1,e),e})),await Pe.runtimeModuleLoaded.promise;const n=async e=>{const t=await e;if(t.buffer){if(!Z[t.behavior]){t.buffer&&"object"==typeof t.buffer||Be(!1,"asset buffer must be array-like or buffer-like or promise of these"),"string"!=typeof t.resolvedUrl&&Be(!1,"resolvedUrl must be string");const e=t.resolvedUrl,o=await t.buffer,n=new Uint8Array(o);pe(t),await Ue.beforeOnRuntimeInitialized.promise,Ue.instantiate_asset(t,e,n)}}else J[t.behavior]?("symbols"===t.behavior&&(await Ue.instantiate_symbols_asset(t),pe(t)),J[t.behavior]&&++Pe.actual_downloaded_assets_count):(t.isOptional||Be(!1,"Expected asset to have the downloaded buffer"),!H[t.behavior]&&G(t)&&Pe.expected_downloaded_assets_count--,!Z[t.behavior]&&G(t)&&Pe.expected_instantiated_assets_count--)},r=[],i=[];for(const t of e)r.push(n(t));for(const e of t)i.push(n(e));Promise.all(r).then((()=>{Ce||Ue.coreAssetsInMemory.promise_control.resolve()})).catch((e=>{throw Pe.err("Error in mono_download_assets: "+e),Xe(1,e),e})),Promise.all(i).then((async()=>{Ce||(await Ue.coreAssetsInMemory.promise,Ue.allAssetsInMemory.promise_control.resolve())})).catch((e=>{throw Pe.err("Error in mono_download_assets: "+e),Xe(1,e),e}))}catch(e){throw Pe.err("Error in mono_download_assets: "+e),e}}}let ne=!1;function re(){if(ne)return;ne=!0;const e=Pe.config,t=[];if(e.assets)for(const t of e.assets)"object"!=typeof t&&Be(!1,`asset must be object, it was ${typeof t} : ${t}`),"string"!=typeof t.behavior&&Be(!1,"asset behavior must be known string"),"string"!=typeof t.name&&Be(!1,"asset name must be string"),t.resolvedUrl&&"string"!=typeof t.resolvedUrl&&Be(!1,"asset resolvedUrl could be string"),t.hash&&"string"!=typeof t.hash&&Be(!1,"asset resolvedUrl could be string"),t.pendingDownload&&"object"!=typeof t.pendingDownload&&Be(!1,"asset pendingDownload could be object"),t.isCore?$.push(t):z.push(t),X(t);else if(e.resources){const o=e.resources;o.wasmNative||Be(!1,"resources.wasmNative must be defined"),o.jsModuleNative||Be(!1,"resources.jsModuleNative must be defined"),o.jsModuleRuntime||Be(!1,"resources.jsModuleRuntime must be defined"),K(z,o.wasmNative,"dotnetwasm"),K(t,o.jsModuleNative,"js-module-native"),K(t,o.jsModuleRuntime,"js-module-runtime"),o.jsModuleDiagnostics&&K(t,o.jsModuleDiagnostics,"js-module-diagnostics");const n=(e,t,o)=>{const n=e;n.behavior=t,o?(n.isCore=!0,$.push(n)):z.push(n)};if(o.coreAssembly)for(let e=0;e<o.coreAssembly.length;e++)n(o.coreAssembly[e],"assembly",!0);if(o.assembly)for(let e=0;e<o.assembly.length;e++)n(o.assembly[e],"assembly",!o.coreAssembly);if(0!=e.debugLevel&&Pe.isDebuggingSupported()){if(o.corePdb)for(let e=0;e<o.corePdb.length;e++)n(o.corePdb[e],"pdb",!0);if(o.pdb)for(let e=0;e<o.pdb.length;e++)n(o.pdb[e],"pdb",!o.corePdb)}if(e.loadAllSatelliteResources&&o.satelliteResources)for(const e in o.satelliteResources)for(let t=0;t<o.satelliteResources[e].length;t++){const r=o.satelliteResources[e][t];r.culture=e,n(r,"resource",!o.coreAssembly)}if(o.coreVfs)for(let e=0;e<o.coreVfs.length;e++)n(o.coreVfs[e],"vfs",!0);if(o.vfs)for(let e=0;e<o.vfs.length;e++)n(o.vfs[e],"vfs",!o.coreVfs);const r=O(e);if(r&&o.icu)for(let e=0;e<o.icu.length;e++){const t=o.icu[e];t.name===r&&n(t,"icu",!1)}if(o.wasmSymbols)for(let e=0;e<o.wasmSymbols.length;e++)n(o.wasmSymbols[e],"symbols",!1)}if(e.appsettings)for(let t=0;t<e.appsettings.length;t++){const o=e.appsettings[t],n=he(o);"appsettings.json"!==n&&n!==`appsettings.${e.applicationEnvironment}.json`||z.push({name:o,behavior:"vfs",cache:"no-cache",useCredentials:!0})}e.assets=[...$,...z,...t]}async function ie(e){const t=await se(e);return await t.pendingDownloadInternal.response,t.buffer}async function se(e){try{return await ae(e)}catch(t){if(!Pe.enableDownloadRetry)throw t;if(Ie||Se)throw t;if(e.pendingDownload&&e.pendingDownloadInternal==e.pendingDownload)throw t;if(e.resolvedUrl&&-1!=e.resolvedUrl.indexOf("file://"))throw t;if(t&&404==t.status)throw t;e.pendingDownloadInternal=void 0,await Pe.allDownloadsQueued.promise;try{return Pe.diagnosticTracing&&b(`Retrying download '${e.name}'`),await ae(e)}catch(t){return e.pendingDownloadInternal=void 0,await new Promise((e=>globalThis.setTimeout(e,100))),Pe.diagnosticTracing&&b(`Retrying download (2) '${e.name}' after delay`),await ae(e)}}}async function ae(e){for(;L;)await L.promise;try{++N,N==Pe.maxParallelDownloads&&(Pe.diagnosticTracing&&b("Throttling further parallel downloads"),L=i());const t=await async function(e){if(e.pendingDownload&&(e.pendingDownloadInternal=e.pendingDownload),e.pendingDownloadInternal&&e.pendingDownloadInternal.response)return e.pendingDownloadInternal.response;if(e.buffer){const t=await e.buffer;return e.resolvedUrl||(e.resolvedUrl="undefined://"+e.name),e.pendingDownloadInternal={url:e.resolvedUrl,name:e.name,response:Promise.resolve({ok:!0,arrayBuffer:()=>t,json:()=>JSON.parse(new TextDecoder("utf-8").decode(t)),text:()=>{throw new Error("NotImplementedException")},headers:{get:()=>{}}})},e.pendingDownloadInternal.response}const t=e.loadRemote&&Pe.config.remoteSources?Pe.config.remoteSources:[""];let o;for(let n of t){n=n.trim(),"./"===n&&(n="");const t=le(e,n);e.name===t?Pe.diagnosticTracing&&b(`Attempting to download '${t}'`):Pe.diagnosticTracing&&b(`Attempting to download '${t}' for ${e.name}`);try{e.resolvedUrl=t;const n=fe(e);if(e.pendingDownloadInternal=n,o=await n.response,!o||!o.ok)continue;return o}catch(e){o||(o={ok:!1,url:t,status:0,statusText:""+e});continue}}const n=e.isOptional||e.name.match(/\.pdb$/)&&Pe.config.ignorePdbLoadErrors;if(o||Be(!1,`Response undefined ${e.name}`),!n){const t=new Error(`download '${o.url}' for ${e.name} failed ${o.status} ${o.statusText}`);throw t.status=o.status,t}y(`optional download '${o.url}' for ${e.name} failed ${o.status} ${o.statusText}`)}(e);return t?(J[e.behavior]||(e.buffer=await t.arrayBuffer(),++Pe.actual_downloaded_assets_count),e):e}finally{if(--N,L&&N==Pe.maxParallelDownloads-1){Pe.diagnosticTracing&&b("Resuming more parallel downloads");const e=L;L=void 0,e.promise_control.resolve()}}}function le(e,t){let o;return null==t&&Be(!1,`sourcePrefix must be provided for ${e.name}`),e.resolvedUrl?o=e.resolvedUrl:(o=""===t?"assembly"===e.behavior||"pdb"===e.behavior?e.name:"resource"===e.behavior&&e.culture&&""!==e.culture?`${e.culture}/${e.name}`:e.name:t+e.name,o=ce(Pe.locateFile(o),e.behavior)),o&&"string"==typeof o||Be(!1,"attemptUrl need to be path or url string"),o}function ce(e,t){return Pe.modulesUniqueQuery&&q[t]&&(e+=Pe.modulesUniqueQuery),e}let de=0;const ue=new Set;function fe(e){try{e.resolvedUrl||Be(!1,"Request's resolvedUrl must be set");const t=function(e){let t=e.resolvedUrl;if(Pe.loadBootResource){const o=ge(e);if(o instanceof Promise)return o;"string"==typeof o&&(t=o)}const o={};return e.cache?o.cache=e.cache:Pe.config.disableNoCacheFetch||(o.cache="no-cache"),e.useCredentials?o.credentials="include":!Pe.config.disableIntegrityCheck&&e.hash&&(o.integrity=e.hash),Pe.fetch_like(t,o)}(e),o={name:e.name,url:e.resolvedUrl,response:t};return ue.add(e.name),o.response.then((()=>{"assembly"==e.behavior&&Pe.loadedAssemblies.push(e.name),de++,Pe.onDownloadResourceProgress&&Pe.onDownloadResourceProgress(de,ue.size)})),o}catch(t){const o={ok:!1,url:e.resolvedUrl,status:500,statusText:"ERR29: "+t,arrayBuffer:()=>{throw t},json:()=>{throw t}};return{name:e.name,url:e.resolvedUrl,response:Promise.resolve(o)}}}const me={resource:"assembly",assembly:"assembly",pdb:"pdb",icu:"globalization",vfs:"configuration",manifest:"manifest",dotnetwasm:"dotnetwasm","js-module-dotnet":"dotnetjs","js-module-native":"dotnetjs","js-module-runtime":"dotnetjs","js-module-threads":"dotnetjs"};function ge(e){var t;if(Pe.loadBootResource){const o=null!==(t=e.hash)&&void 0!==t?t:"",n=e.resolvedUrl,r=me[e.behavior];if(r){const t=Pe.loadBootResource(r,e.name,n,o,e.behavior);return"string"==typeof t?I(t):t}}}function pe(e){e.pendingDownloadInternal=null,e.pendingDownload=null,e.buffer=null,e.moduleExports=null}function he(e){let t=e.lastIndexOf("/");return t>=0&&t++,e.substring(t)}async function we(e){e&&await Promise.all((null!=e?e:[]).map((e=>async function(e){try{const t=e.name;if(!e.moduleExports){const o=ce(Pe.locateFile(t),"js-module-library-initializer");Pe.diagnosticTracing&&b(`Attempting to import '${o}' for ${e}`),e.moduleExports=await import(/*! webpackIgnore: true */o)}Pe.libraryInitializers.push({scriptName:t,exports:e.moduleExports})}catch(t){E(`Failed to import library initializer '${e}': ${t}`)}}(e))))}async function be(e,t){if(!Pe.libraryInitializers)return;const o=[];for(let n=0;n<Pe.libraryInitializers.length;n++){const r=Pe.libraryInitializers[n];r.exports[e]&&o.push(ye(r.scriptName,e,(()=>r.exports[e](...t))))}await Promise.all(o)}async function ye(e,t,o){try{await o()}catch(o){throw E(`Failed to invoke '${t}' on library initializer '${e}': ${o}`),Xe(1,o),o}}function ve(e,t){if(e===t)return e;const o={...t};return void 0!==o.assets&&o.assets!==e.assets&&(o.assets=[...e.assets||[],...o.assets||[]]),void 0!==o.resources&&(o.resources=_e(e.resources||{assembly:[],jsModuleNative:[],jsModuleRuntime:[],wasmNative:[]},o.resources)),void 0!==o.environmentVariables&&(o.environmentVariables={...e.environmentVariables||{},...o.environmentVariables||{}}),void 0!==o.runtimeOptions&&o.runtimeOptions!==e.runtimeOptions&&(o.runtimeOptions=[...e.runtimeOptions||[],...o.runtimeOptions||[]]),Object.assign(e,o)}function Ee(e,t){if(e===t)return e;const o={...t};return o.config&&(e.config||(e.config={}),o.config=ve(e.config,o.config)),Object.assign(e,o)}function _e(e,t){if(e===t)return e;const o={...t};return void 0!==o.coreAssembly&&(o.coreAssembly=[...e.coreAssembly||[],...o.coreAssembly||[]]),void 0!==o.assembly&&(o.assembly=[...e.assembly||[],...o.assembly||[]]),void 0!==o.lazyAssembly&&(o.lazyAssembly=[...e.lazyAssembly||[],...o.lazyAssembly||[]]),void 0!==o.corePdb&&(o.corePdb=[...e.corePdb||[],...o.corePdb||[]]),void 0!==o.pdb&&(o.pdb=[...e.pdb||[],...o.pdb||[]]),void 0!==o.jsModuleWorker&&(o.jsModuleWorker=[...e.jsModuleWorker||[],...o.jsModuleWorker||[]]),void 0!==o.jsModuleNative&&(o.jsModuleNative=[...e.jsModuleNative||[],...o.jsModuleNative||[]]),void 0!==o.jsModuleDiagnostics&&(o.jsModuleDiagnostics=[...e.jsModuleDiagnostics||[],...o.jsModuleDiagnostics||[]]),void 0!==o.jsModuleRuntime&&(o.jsModuleRuntime=[...e.jsModuleRuntime||[],...o.jsModuleRuntime||[]]),void 0!==o.wasmSymbols&&(o.wasmSymbols=[...e.wasmSymbols||[],...o.wasmSymbols||[]]),void 0!==o.wasmNative&&(o.wasmNative=[...e.wasmNative||[],...o.wasmNative||[]]),void 0!==o.icu&&(o.icu=[...e.icu||[],...o.icu||[]]),void 0!==o.satelliteResources&&(o.satelliteResources=function(e,t){if(e===t)return e;for(const o in t)e[o]=[...e[o]||[],...t[o]||[]];return e}(e.satelliteResources||{},o.satelliteResources||{})),void 0!==o.modulesAfterConfigLoaded&&(o.modulesAfterConfigLoaded=[...e.modulesAfterConfigLoaded||[],...o.modulesAfterConfigLoaded||[]]),void 0!==o.modulesAfterRuntimeReady&&(o.modulesAfterRuntimeReady=[...e.modulesAfterRuntimeReady||[],...o.modulesAfterRuntimeReady||[]]),void 0!==o.extensions&&(o.extensions={...e.extensions||{},...o.extensions||{}}),void 0!==o.vfs&&(o.vfs=[...e.vfs||[],...o.vfs||[]]),Object.assign(e,o)}function xe(){const e=Pe.config;if(e.environmentVariables=e.environmentVariables||{},e.runtimeOptions=e.runtimeOptions||[],e.resources=e.resources||{assembly:[],jsModuleNative:[],jsModuleWorker:[],jsModuleRuntime:[],wasmNative:[],vfs:[],satelliteResources:{}},e.assets){Pe.diagnosticTracing&&b("config.assets is deprecated, use config.resources instead");for(const t of e.assets){const o={};switch(t.behavior){case"assembly":o.assembly=[t];break;case"pdb":o.pdb=[t];break;case"resource":o.satelliteResources={},o.satelliteResources[t.culture]=[t];break;case"icu":o.icu=[t];break;case"symbols":o.wasmSymbols=[t];break;case"vfs":o.vfs=[t];break;case"dotnetwasm":o.wasmNative=[t];break;case"js-module-threads":o.jsModuleWorker=[t];break;case"js-module-runtime":o.jsModuleRuntime=[t];break;case"js-module-native":o.jsModuleNative=[t];break;case"js-module-diagnostics":o.jsModuleDiagnostics=[t];break;case"js-module-dotnet":break;default:throw new Error(`Unexpected behavior ${t.behavior} of asset ${t.name}`)}_e(e.resources,o)}}e.debugLevel,e.applicationEnvironment||(e.applicationEnvironment="Production"),e.applicationCulture&&(e.environmentVariables.LANG=`${e.applicationCulture}.UTF-8`),Ue.diagnosticTracing=Pe.diagnosticTracing=!!e.diagnosticTracing,Ue.waitForDebugger=e.waitForDebugger,Pe.maxParallelDownloads=e.maxParallelDownloads||Pe.maxParallelDownloads,Pe.enableDownloadRetry=void 0!==e.enableDownloadRetry?e.enableDownloadRetry:Pe.enableDownloadRetry}let je=!1;async function Re(e){var t;if(je)return void await Pe.afterConfigLoaded.promise;let o;try{if(e.configSrc||Pe.config&&0!==Object.keys(Pe.config).length&&(Pe.config.assets||Pe.config.resources)||(e.configSrc="dotnet.boot.js"),o=e.configSrc,je=!0,o&&(Pe.diagnosticTracing&&b("mono_wasm_load_config"),await async function(e){const t=e.configSrc,o=Pe.locateFile(t);let n=null;void 0!==Pe.loadBootResource&&(n=Pe.loadBootResource("manifest",t,o,"","manifest"));let r,i=null;if(n)if("string"==typeof n)n.includes(".json")?(i=await s(I(n)),r=await Ae(i)):r=(await import(I(n))).config;else{const e=await n;"function"==typeof e.json?(i=e,r=await Ae(i)):r=e.config}else o.includes(".json")?(i=await s(ce(o,"manifest")),r=await Ae(i)):r=(await import(ce(o,"manifest"))).config;function s(e){return Pe.fetch_like(e,{method:"GET",credentials:"include",cache:"no-cache"})}Pe.config.applicationEnvironment&&(r.applicationEnvironment=Pe.config.applicationEnvironment),ve(Pe.config,r)}(e)),xe(),await we(null===(t=Pe.config.resources)||void 0===t?void 0:t.modulesAfterConfigLoaded),await be("onRuntimeConfigLoaded",[Pe.config]),e.onConfigLoaded)try{await e.onConfigLoaded(Pe.config,Le),xe()}catch(e){throw _("onConfigLoaded() failed",e),e}xe(),Pe.afterConfigLoaded.promise_control.resolve(Pe.config)}catch(t){const n=`Failed to load config file ${o} ${t} ${null==t?void 0:t.stack}`;throw Pe.config=e.config=Object.assign(Pe.config,{message:n,error:t,isError:!0}),Xe(1,new Error(n)),t}}function Te(){return!!globalThis.navigator&&(Pe.isChromium||Pe.isFirefox)}async function Ae(e){const t=Pe.config,o=await e.json();t.applicationEnvironment||o.applicationEnvironment||(o.applicationEnvironment=e.headers.get("Blazor-Environment")||e.headers.get("DotNet-Environment")||void 0),o.environmentVariables||(o.environmentVariables={});const n=e.headers.get("DOTNET-MODIFIABLE-ASSEMBLIES");n&&(o.environmentVariables.DOTNET_MODIFIABLE_ASSEMBLIES=n);const r=e.headers.get("ASPNETCORE-BROWSER-TOOLS");return r&&(o.environmentVariables.__ASPNETCORE_BROWSER_TOOLS=r),o}"function"!=typeof importScripts||globalThis.onmessage||(globalThis.dotnetSidecar=!0);const Se="object"==typeof process&&"object"==typeof process.versions&&"string"==typeof process.versions.node,De="function"==typeof importScripts,Oe=De&&"undefined"!=typeof dotnetSidecar,Ce=De&&!Oe,ke="object"==typeof window||De&&!Se,Ie=!ke&&!Se;let Ue={},Pe={},Me={},Le={},Ne={},$e=!1;const ze={},We={config:ze},Fe={mono:{},binding:{},internal:Ne,module:We,loaderHelpers:Pe,runtimeHelpers:Ue,diagnosticHelpers:Me,api:Le};function Be(e,t){if(e)return;const o="Assert failed: "+("function"==typeof t?t():t),n=new Error(o);_(o,n),Ue.nativeAbort(n)}function Ve(){return void 0!==Pe.exitCode}function qe(){return Ue.runtimeReady&&!Ve()}function He(){Ve()&&Be(!1,`.NET runtime already exited with ${Pe.exitCode} ${Pe.exitReason}. You can use runtime.runMain() which doesn't exit the runtime.`),Ue.runtimeReady||Be(!1,".NET runtime didn't start yet. Please call dotnet.create() first.")}function Je(){ke&&(globalThis.addEventListener("unhandledrejection",et),globalThis.addEventListener("error",tt))}let Ze,Qe;function Ge(e){Qe&&Qe(e),Xe(e,Pe.exitReason)}function Ke(e){Ze&&Ze(e||Pe.exitReason),Xe(1,e||Pe.exitReason)}function Xe(t,o){var n,r;const i=o&&"object"==typeof o;t=i&&"number"==typeof o.status?o.status:void 0===t?-1:t;const s=i&&"string"==typeof o.message?o.message:""+o;(o=i?o:Ue.ExitStatus?function(e,t){const o=new Ue.ExitStatus(e);return o.message=t,o.toString=()=>t,o}(t,s):new Error("Exit with code "+t+" "+s)).status=t,o.message||(o.message=s);const a=""+(o.stack||(new Error).stack);try{Object.defineProperty(o,"stack",{get:()=>a})}catch(e){}const l=!!o.silent;if(o.silent=!0,Ve())Pe.diagnosticTracing&&b("mono_exit called after exit");else{try{We.onAbort==Ke&&(We.onAbort=Ze),We.onExit==Ge&&(We.onExit=Qe),ke&&(globalThis.removeEventListener("unhandledrejection",et),globalThis.removeEventListener("error",tt)),Ue.runtimeReady?(Ue.jiterpreter_dump_stats&&Ue.jiterpreter_dump_stats(!1),0===t&&(null===(n=Pe.config)||void 0===n?void 0:n.interopCleanupOnExit)&&Ue.forceDisposeProxies(!0,!0),e&&0!==t&&(null===(r=Pe.config)||void 0===r||r.dumpThreadsOnNonZeroExit)):(Pe.diagnosticTracing&&b(`abort_startup, reason: ${o}`),function(e){Pe.allDownloadsQueued.promise_control.reject(e),Pe.allDownloadsFinished.promise_control.reject(e),Pe.afterConfigLoaded.promise_control.reject(e),Pe.wasmCompilePromise.promise_control.reject(e),Pe.runtimeModuleLoaded.promise_control.reject(e),Ue.dotnetReady&&(Ue.dotnetReady.promise_control.reject(e),Ue.afterInstantiateWasm.promise_control.reject(e),Ue.beforePreInit.promise_control.reject(e),Ue.afterPreInit.promise_control.reject(e),Ue.afterPreRun.promise_control.reject(e),Ue.beforeOnRuntimeInitialized.promise_control.reject(e),Ue.afterOnRuntimeInitialized.promise_control.reject(e),Ue.afterPostRun.promise_control.reject(e))}(o))}catch(e){E("mono_exit A failed",e)}try{l||(function(e,t){if(0!==e&&t){const e=Ue.ExitStatus&&t instanceof Ue.ExitStatus?b:_;"string"==typeof t?e(t):(void 0===t.stack&&(t.stack=(new Error).stack+""),t.message?e(Ue.stringify_as_error_with_stack?Ue.stringify_as_error_with_stack(t.message+"\n"+t.stack):t.message+"\n"+t.stack):e(JSON.stringify(t)))}!Ce&&Pe.config&&(Pe.config.logExitCode?Pe.config.forwardConsoleLogsToWS?R("WASM EXIT "+e):v("WASM EXIT "+e):Pe.config.forwardConsoleLogsToWS&&R())}(t,o),function(e){if(ke&&!Ce&&Pe.config&&Pe.config.appendElementOnExit&&document){const t=document.createElement("label");t.id="tests_done",0!==e&&(t.style.background="red"),t.innerHTML=""+e,document.body.appendChild(t)}}(t))}catch(e){E("mono_exit B failed",e)}Pe.exitCode=t,Pe.exitReason||(Pe.exitReason=o),!Ce&&Ue.runtimeReady&&We.runtimeKeepalivePop()}if(Pe.config&&Pe.config.asyncFlushOnExit&&0===t)throw(async()=>{try{await async function(){try{const e=await import(/*! webpackIgnore: true */"process"),t=e=>new Promise(((t,o)=>{e.on("error",o),e.end("","utf8",t)})),o=t(e.stderr),n=t(e.stdout);let r;const i=new Promise((e=>{r=setTimeout((()=>e("timeout")),1e3)}));await Promise.race([Promise.all([n,o]),i]),clearTimeout(r)}catch(e){_(`flushing std* streams failed: ${e}`)}}()}finally{Ye(t,o)}})(),o;Ye(t,o)}function Ye(e,t){if(Ue.runtimeReady&&Ue.nativeExit)try{Ue.nativeExit(e)}catch(e){!Ue.ExitStatus||e instanceof Ue.ExitStatus||E("set_exit_code_and_quit_now failed: "+e.toString())}if(0!==e||!ke)throw Se&&Ne.process?Ne.process.exit(e):Ue.quit&&Ue.quit(e,t),t}function et(e){ot(e,e.reason,"rejection")}function tt(e){ot(e,e.error,"error")}function ot(e,t,o){e.preventDefault();try{t||(t=new Error("Unhandled "+o)),void 0===t.stack&&(t.stack=(new Error).stack),t.stack=t.stack+"",t.silent||(_("Unhandled error:",t),Xe(1,t))}catch(e){}}!function(e){if($e)throw new Error("Loader module already loaded");$e=!0,Ue=e.runtimeHelpers,Pe=e.loaderHelpers,Me=e.diagnosticHelpers,Le=e.api,Ne=e.internal,Object.assign(Le,{INTERNAL:Ne,invokeLibraryInitializers:be}),Object.assign(e.module,{config:ve(ze,{environmentVariables:{}})});const r={mono_wasm_bindings_is_ready:!1,config:e.module.config,diagnosticTracing:!1,nativeAbort:e=>{throw e||new Error("abort")},nativeExit:e=>{throw new Error("exit:"+e)}},l={gitHash:"e2f47b0110ed922f21a1522da67279133ce28f32",config:e.module.config,diagnosticTracing:!1,maxParallelDownloads:16,enableDownloadRetry:!0,_loaded_files:[],loadedFiles:[],loadedAssemblies:[],libraryInitializers:[],workerNextNumber:1,actual_downloaded_assets_count:0,actual_instantiated_assets_count:0,expected_downloaded_assets_count:0,expected_instantiated_assets_count:0,afterConfigLoaded:i(),allDownloadsQueued:i(),allDownloadsFinished:i(),wasmCompilePromise:i(),runtimeModuleLoaded:i(),loadingWorkers:i(),is_exited:Ve,is_runtime_running:qe,assert_runtime_running:He,mono_exit:Xe,createPromiseController:i,getPromiseController:s,assertIsControllablePromise:a,mono_download_assets:oe,resolve_single_asset_path:ee,setup_proxy_console:j,set_thread_prefix:w,installUnhandledErrorHandler:Je,retrieve_asset_download:ie,invokeLibraryInitializers:be,isDebuggingSupported:Te,exceptions:t,simd:n,relaxedSimd:o};Object.assign(Ue,r),Object.assign(Pe,l)}(Fe);let nt,rt,it,st=!1,at=!1;async function lt(e){if(!at){if(at=!0,ke&&Pe.config.forwardConsoleLogsToWS&&void 0!==globalThis.WebSocket&&j("main",globalThis.console,globalThis.location.origin),We||Be(!1,"Null moduleConfig"),Pe.config||Be(!1,"Null moduleConfig.config"),"function"==typeof e){const t=e(Fe.api);if(t.ready)throw new Error("Module.ready couldn't be redefined.");Object.assign(We,t),Ee(We,t)}else{if("object"!=typeof e)throw new Error("Can't use moduleFactory callback of createDotnetRuntime function.");Ee(We,e)}await async function(e){if(Se){const e=await import(/*! webpackIgnore: true */"process"),t=14;if(e.versions.node.split(".")[0]<t)throw new Error(`NodeJS at '${e.execPath}' has too low version '${e.versions.node}', please use at least ${t}. See also https://aka.ms/dotnet-wasm-features`)}const t=/*! webpackIgnore: true */import.meta.url,o=t.indexOf("?");var n;if(o>0&&(Pe.modulesUniqueQuery=t.substring(o)),Pe.scriptUrl=t.replace(/\\/g,"/").replace(/[?#].*/,""),Pe.scriptDirectory=(n=Pe.scriptUrl).slice(0,n.lastIndexOf("/"))+"/",Pe.locateFile=e=>"URL"in globalThis&&globalThis.URL!==C?new URL(e,Pe.scriptDirectory).toString():M(e)?e:Pe.scriptDirectory+e,Pe.fetch_like=k,Pe.out=console.log,Pe.err=console.error,Pe.onDownloadResourceProgress=e.onDownloadResourceProgress,ke&&globalThis.navigator){const e=globalThis.navigator,t=e.userAgentData&&e.userAgentData.brands;t&&t.length>0?Pe.isChromium=t.some((e=>"Google Chrome"===e.brand||"Microsoft Edge"===e.brand||"Chromium"===e.brand)):e.userAgent&&(Pe.isChromium=e.userAgent.includes("Chrome"),Pe.isFirefox=e.userAgent.includes("Firefox"))}Ne.require=Se?await import(/*! webpackIgnore: true */"module").then((e=>e.createRequire(/*! webpackIgnore: true */import.meta.url))):Promise.resolve((()=>{throw new Error("require not supported")})),void 0===globalThis.URL&&(globalThis.URL=C)}(We)}}async function ct(e){return await lt(e),Ze=We.onAbort,Qe=We.onExit,We.onAbort=Ke,We.onExit=Ge,We.ENVIRONMENT_IS_PTHREAD?async function(){(function(){const e=new MessageChannel,t=e.port1,o=e.port2;t.addEventListener("message",(e=>{var n,r;n=JSON.parse(e.data.config),r=JSON.parse(e.data.monoThreadInfo),st?Pe.diagnosticTracing&&b("mono config already received"):(ve(Pe.config,n),Ue.monoThreadInfo=r,xe(),Pe.diagnosticTracing&&b("mono config received"),st=!0,Pe.afterConfigLoaded.promise_control.resolve(Pe.config),ke&&n.forwardConsoleLogsToWS&&void 0!==globalThis.WebSocket&&Pe.setup_proxy_console("worker-idle",console,globalThis.location.origin)),t.close(),o.close()}),{once:!0}),t.start(),self.postMessage({[l]:{monoCmd:"preload",port:o}},[o])})(),await Pe.afterConfigLoaded.promise,function(){const e=Pe.config;e.assets||Be(!1,"config.assets must be defined");for(const t of e.assets)X(t),Q[t.behavior]&&z.push(t)}(),setTimeout((async()=>{try{await oe()}catch(e){Xe(1,e)}}),0);const e=dt(),t=await Promise.all(e);return await ut(t),We}():async function(){var e;await Re(We),re();const t=dt();(async function(){try{const e=ee("dotnetwasm");await se(e),e&&e.pendingDownloadInternal&&e.pendingDownloadInternal.response||Be(!1,"Can't load dotnet.native.wasm");const t=await e.pendingDownloadInternal.response,o=t.headers&&t.headers.get?t.headers.get("Content-Type"):void 0;let n;if("function"==typeof WebAssembly.compileStreaming&&"application/wasm"===o)n=await WebAssembly.compileStreaming(t);else{ke&&"application/wasm"!==o&&E('WebAssembly resource does not have the expected content type "application/wasm", so falling back to slower ArrayBuffer instantiation.');const e=await t.arrayBuffer();Pe.diagnosticTracing&&b("instantiate_wasm_module buffered"),n=Ie?await Promise.resolve(new WebAssembly.Module(e)):await WebAssembly.compile(e)}e.pendingDownloadInternal=null,e.pendingDownload=null,e.buffer=null,e.moduleExports=null,Pe.wasmCompilePromise.promise_control.resolve(n)}catch(e){Pe.wasmCompilePromise.promise_control.reject(e)}})(),setTimeout((async()=>{try{D(),await oe()}catch(e){Xe(1,e)}}),0);const o=await Promise.all(t);return await ut(o),await Ue.dotnetReady.promise,await we(null===(e=Pe.config.resources)||void 0===e?void 0:e.modulesAfterRuntimeReady),await be("onRuntimeReady",[Fe.api]),Le}()}function dt(){const e=ee("js-module-runtime"),t=ee("js-module-native");if(nt&&rt)return[nt,rt,it];"object"==typeof e.moduleExports?nt=e.moduleExports:(Pe.diagnosticTracing&&b(`Attempting to import '${e.resolvedUrl}' for ${e.name}`),nt=import(/*! webpackIgnore: true */e.resolvedUrl)),"object"==typeof t.moduleExports?rt=t.moduleExports:(Pe.diagnosticTracing&&b(`Attempting to import '${t.resolvedUrl}' for ${t.name}`),rt=import(/*! webpackIgnore: true */t.resolvedUrl));const o=Y("js-module-diagnostics");return o&&("object"==typeof o.moduleExports?it=o.moduleExports:(Pe.diagnosticTracing&&b(`Attempting to import '${o.resolvedUrl}' for ${o.name}`),it=import(/*! webpackIgnore: true */o.resolvedUrl))),[nt,rt,it]}async function ut(e){const{initializeExports:t,initializeReplacements:o,configureRuntimeStartup:n,configureEmscriptenStartup:r,configureWorkerStartup:i,setRuntimeGlobals:s,passEmscriptenInternals:a}=e[0],{default:l}=e[1],c=e[2];s(Fe),t(Fe),c&&c.setRuntimeGlobals(Fe),await n(We),Pe.runtimeModuleLoaded.promise_control.resolve(),l((e=>(Object.assign(We,{ready:e.ready,__dotnet_runtime:{initializeReplacements:o,configureEmscriptenStartup:r,configureWorkerStartup:i,passEmscriptenInternals:a}}),We))).catch((e=>{if(e.message&&e.message.toLowerCase().includes("out of memory"))throw new Error(".NET runtime has failed to start, because too much memory was requested. Please decrease the memory by adjusting EmccMaximumHeapSize. See also https://aka.ms/dotnet-wasm-features");throw e}))}const ft=new class{withModuleConfig(e){try{return Ee(We,e),this}catch(e){throw Xe(1,e),e}}withOnConfigLoaded(e){try{return Ee(We,{onConfigLoaded:e}),this}catch(e){throw Xe(1,e),e}}withConsoleForwarding(){try{return ve(ze,{forwardConsoleLogsToWS:!0}),this}catch(e){throw Xe(1,e),e}}withExitOnUnhandledError(){try{return ve(ze,{exitOnUnhandledError:!0}),Je(),this}catch(e){throw Xe(1,e),e}}withAsyncFlushOnExit(){try{return ve(ze,{asyncFlushOnExit:!0}),this}catch(e){throw Xe(1,e),e}}withExitCodeLogging(){try{return ve(ze,{logExitCode:!0}),this}catch(e){throw Xe(1,e),e}}withElementOnExit(){try{return ve(ze,{appendElementOnExit:!0}),this}catch(e){throw Xe(1,e),e}}withInteropCleanupOnExit(){try{return ve(ze,{interopCleanupOnExit:!0}),this}catch(e){throw Xe(1,e),e}}withDumpThreadsOnNonZeroExit(){try{return ve(ze,{dumpThreadsOnNonZeroExit:!0}),this}catch(e){throw Xe(1,e),e}}withWaitingForDebugger(e){try{return ve(ze,{waitForDebugger:e}),this}catch(e){throw Xe(1,e),e}}withInterpreterPgo(e,t){try{return ve(ze,{interpreterPgo:e,interpreterPgoSaveDelay:t}),ze.runtimeOptions?ze.runtimeOptions.push("--interp-pgo-recording"):ze.runtimeOptions=["--interp-pgo-recording"],this}catch(e){throw Xe(1,e),e}}withConfig(e){try{return ve(ze,e),this}catch(e){throw Xe(1,e),e}}withConfigSrc(e){try{return e&&"string"==typeof e||Be(!1,"must be file path or URL"),Ee(We,{configSrc:e}),this}catch(e){throw Xe(1,e),e}}withVirtualWorkingDirectory(e){try{return e&&"string"==typeof e||Be(!1,"must be directory path"),ve(ze,{virtualWorkingDirectory:e}),this}catch(e){throw Xe(1,e),e}}withEnvironmentVariable(e,t){try{const o={};return o[e]=t,ve(ze,{environmentVariables:o}),this}catch(e){throw Xe(1,e),e}}withEnvironmentVariables(e){try{return e&&"object"==typeof e||Be(!1,"must be dictionary object"),ve(ze,{environmentVariables:e}),this}catch(e){throw Xe(1,e),e}}withDiagnosticTracing(e){try{return"boolean"!=typeof e&&Be(!1,"must be boolean"),ve(ze,{diagnosticTracing:e}),this}catch(e){throw Xe(1,e),e}}withDebugging(e){try{return null!=e&&"number"==typeof e||Be(!1,"must be number"),ve(ze,{debugLevel:e}),this}catch(e){throw Xe(1,e),e}}withApplicationArguments(...e){try{return e&&Array.isArray(e)||Be(!1,"must be array of strings"),ve(ze,{applicationArguments:e}),this}catch(e){throw Xe(1,e),e}}withRuntimeOptions(e){try{return e&&Array.isArray(e)||Be(!1,"must be array of strings"),ze.runtimeOptions?ze.runtimeOptions.push(...e):ze.runtimeOptions=e,this}catch(e){throw Xe(1,e),e}}withMainAssembly(e){try{return ve(ze,{mainAssemblyName:e}),this}catch(e){throw Xe(1,e),e}}withApplicationArgumentsFromQuery(){try{if(!globalThis.window)throw new Error("Missing window to the query parameters from");if(void 0===globalThis.URLSearchParams)throw new Error("URLSearchParams is supported");const e=new URLSearchParams(globalThis.window.location.search).getAll("arg");return this.withApplicationArguments(...e)}catch(e){throw Xe(1,e),e}}withApplicationEnvironment(e){try{return ve(ze,{applicationEnvironment:e}),this}catch(e){throw Xe(1,e),e}}withApplicationCulture(e){try{return ve(ze,{applicationCulture:e}),this}catch(e){throw Xe(1,e),e}}withResourceLoader(e){try{return Pe.loadBootResource=e,this}catch(e){throw Xe(1,e),e}}async download(){try{await async function(){lt(We),await Re(We),re(),D(),oe(),await Pe.allDownloadsFinished.promise}()}catch(e){throw Xe(1,e),e}}async create(){try{return this.instance||(this.instance=await async function(){return await ct(We),Fe.api}()),this.instance}catch(e){throw Xe(1,e),e}}async run(){try{return We.config||Be(!1,"Null moduleConfig.config"),this.instance||await this.create(),this.instance.runMainAndExit()}catch(e){throw Xe(1,e),e}}},mt=Xe,gt=ct;Ie||"function"==typeof globalThis.URL||Be(!1,"This browser/engine doesn't support URL API. Please use a modern version. See also https://aka.ms/dotnet-wasm-features"),"function"!=typeof globalThis.BigInt64Array&&Be(!1,"This browser/engine doesn't support BigInt64Array API. Please use a modern version. See also https://aka.ms/dotnet-wasm-features"),ft.withConfig(/*json-start*/{
  "mainAssemblyName": "Cursor_Installer_Creator.Browser",
  "resources": {
    "hash": "sha256-nsY9knNT6t7H/2zC+ZvbVoJhBk8ifOVnUmAPFde5ARU=",
    "jsModuleNative": [
      {
        "name": "dotnet.native.93016im1eg.js"
      }
    ],
    "jsModuleRuntime": [
      {
        "name": "dotnet.runtime.zbexyp8zrs.js"
      }
    ],
    "wasmNative": [
      {
        "name": "dotnet.native.k2iscknyto.wasm",
        "hash": "sha256-hGkn2krkhq+mAZmmI7RXgIUsYAf3AlBuV/QPvY2837I=",
        "cache": "force-cache"
      }
    ],
    "icu": [
      {
        "virtualPath": "icudt_CJK.dat",
        "name": "icudt_CJK.tjcz0u77k5.dat",
        "hash": "sha256-SZLtQnRc0JkwqHab0VUVP7T3uBPSeYzxzDnpxPpUnHk=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "icudt_EFIGS.dat",
        "name": "icudt_EFIGS.tptq2av103.dat",
        "hash": "sha256-8fItetYY8kQ0ww6oxwTLiT3oXlBwHKumbeP2pRF4yTc=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "icudt_no_CJK.dat",
        "name": "icudt_no_CJK.lfu7j35m59.dat",
        "hash": "sha256-L7sV7NEYP37/Qr2FPCePo5cJqRgTXRwGHuwF5Q+0Nfs=",
        "cache": "force-cache"
      }
    ],
    "coreAssembly": [
      {
        "virtualPath": "System.Private.CoreLib.wasm",
        "name": "System.Private.CoreLib.acqhqif0l6.wasm",
        "hash": "sha256-Zo7KLnYA4xy8mxDzssMreXZfTlHBgRKnLWfYFMiQvCw=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Runtime.InteropServices.JavaScript.wasm",
        "name": "System.Runtime.InteropServices.JavaScript.11d6720mn8.wasm",
        "hash": "sha256-i0mCdwRLNy+fsELohj+iSdXW5CsN1dlkMsDHnTJw5sM=",
        "cache": "force-cache"
      }
    ],
    "assembly": [
      {
        "virtualPath": "Ani.Reader.wasm",
        "name": "Ani.Reader.339l68nrhr.wasm",
        "hash": "sha256-wch5YORn8P3Tu/f2lKqM6iLPrugQanVD3ItIhQuUcJY=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.Base.wasm",
        "name": "Avalonia.Base.1wwzy2kcx1.wasm",
        "hash": "sha256-v4S8S6jhb0DuEJwFvHyvc3qRNunbZ4Cerr4G2f+2xCQ=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.Browser.wasm",
        "name": "Avalonia.Browser.npyo138c0v.wasm",
        "hash": "sha256-RS0FAKdv0gHD0INnx3L/iFismW7XSnPr/xVonWveHiY=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.Controls.wasm",
        "name": "Avalonia.Controls.xy0mkxadpj.wasm",
        "hash": "sha256-l7lLi6CCZGCWztfQv6YADppNyy+w5YLaerj80iWoKXk=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.DesignerSupport.wasm",
        "name": "Avalonia.DesignerSupport.l3ixncwh27.wasm",
        "hash": "sha256-H7/YoyxbVIy7hv21aXhZiTZed57EPDA8LOB79EGurq4=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.Dialogs.wasm",
        "name": "Avalonia.Dialogs.nlxf575rlr.wasm",
        "hash": "sha256-2wiDbTn2f09Autwtd188+YxiV3FdULK58EpmBdZuY0k=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.Fonts.Inter.wasm",
        "name": "Avalonia.Fonts.Inter.qfgnjk30eg.wasm",
        "hash": "sha256-ClpoNnU65h35OyCJe8sNJ8+d4s1FG+nll3aHEV4DxKg=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.HarfBuzz.wasm",
        "name": "Avalonia.HarfBuzz.ysvha5qh5b.wasm",
        "hash": "sha256-Qhn9KOIcj34S9so8NQ/4o1FCgDkJ+7QSgdYS8UCa4k0=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.Markup.Xaml.wasm",
        "name": "Avalonia.Markup.Xaml.thf0h2x2ka.wasm",
        "hash": "sha256-cdfyImxo2whUZ+qpCbfU5+3Z10vyc3ipsjru4UZo4NM=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.Markup.wasm",
        "name": "Avalonia.Markup.pl1zfsdinb.wasm",
        "hash": "sha256-3aG6jC5dk8pcqCGXT/Gj0k1/b/qPHHUpXJp1Q/J57KM=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.Metal.wasm",
        "name": "Avalonia.Metal.6m4q6spwfj.wasm",
        "hash": "sha256-IiAucO1ub9uokrzDMlhKyT4hjeZgfuREgHmQ5dX/xoA=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.MicroCom.wasm",
        "name": "Avalonia.MicroCom.jaqh4ajcm2.wasm",
        "hash": "sha256-W7tEtNF4g8hdxNEi0bC2oHVJbAmkCky07WKkotXjWmA=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.OpenGL.wasm",
        "name": "Avalonia.OpenGL.kazovu5t7v.wasm",
        "hash": "sha256-VPAoufYOAcux9SSjhodnL+LBtXGww3ulOj6ZopQq88c=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.Remote.Protocol.wasm",
        "name": "Avalonia.Remote.Protocol.vqfrepas9f.wasm",
        "hash": "sha256-d/oevvq3QoB1lezKSyj2kqJVa//xjOJmIheeQ92IB78=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.Skia.wasm",
        "name": "Avalonia.Skia.m9ivmezgiu.wasm",
        "hash": "sha256-xpND7PY3b3f5IvKdXjsbWNwem4n+g3fCKfVVyi5BBC4=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.Themes.Fluent.wasm",
        "name": "Avalonia.Themes.Fluent.6n8h39sf75.wasm",
        "hash": "sha256-Am1vEpgk3+sOAT250Fl/jaBBrqldmoJHhj3yjvfrseQ=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.Vulkan.wasm",
        "name": "Avalonia.Vulkan.l1ylp6pmaz.wasm",
        "hash": "sha256-mUAIJN187+1MVOmtFu++Ht9gNzqpPqSndDKtzF+W0tM=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Avalonia.wasm",
        "name": "Avalonia.npkq25rpvv.wasm",
        "hash": "sha256-W1FheViUxHVcifbKgKtZkGE316FI44J84SRFai2baSY=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "CommonShims.wasm",
        "name": "CommonShims.fz4nlyxyev.wasm",
        "hash": "sha256-Bfo7zmS0MsGYSCvmoHuNdY6l0MpTjzq7dtVCWMGgQkA=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "CommunityToolkit.Mvvm.wasm",
        "name": "CommunityToolkit.Mvvm.7gmxmsktu4.wasm",
        "hash": "sha256-5zW+GBaJU3Yx7rmRTIROwNk92oqs1+nRVbHJ1xleJRQ=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "CsvHelper.wasm",
        "name": "CsvHelper.4zhysgmj1f.wasm",
        "hash": "sha256-gIv3rkSTS7t+R5bMtcyMpbn3EuECRPgHZmWECRjdcd4=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Cursor_Installer_Creator.Browser.wasm",
        "name": "Cursor_Installer_Creator.Browser.ip89agb8pa.wasm",
        "hash": "sha256-Vs5ZLS8CdmZAGG7d5HZLhx35Zac6RggHNFCunWiXGf4=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Cursor_Installer_Creator.wasm",
        "name": "Cursor_Installer_Creator.n3p10n5wn5.wasm",
        "hash": "sha256-rb77/U9Z39aKqF9QU+Iyh/cAdVH9y5btkYPcuLb9n6I=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "ExCSS.wasm",
        "name": "ExCSS.jbixd2k5bc.wasm",
        "hash": "sha256-ATAW0isJpz7mjLGpEjkTzbHywbCFEJIXm3eAaOFzF7g=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "HarfBuzzSharp.wasm",
        "name": "HarfBuzzSharp.skj2rjd285.wasm",
        "hash": "sha256-ZLlju1KLIdabrxDX12d4wrYNpCKTakVe2knm60pqduY=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Ico.Reader.wasm",
        "name": "Ico.Reader.l4g3de5jz9.wasm",
        "hash": "sha256-m1aqrXLH1kuuZfX59iGr/dVpk1SxKXHDQsaajN4P9W8=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "LoadingIndicators.Avalonia.wasm",
        "name": "LoadingIndicators.Avalonia.s70daawd9y.wasm",
        "hash": "sha256-WDQQeL4pBMk5mj5ojOrmf/Vx3DJXwI0a1Lt7t4soQuk=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Magick.NET-Q16-AnyCPU.wasm",
        "name": "Magick.NET-Q16-AnyCPU.6gbrm4gu9g.wasm",
        "hash": "sha256-tlh35K2yvqUh39aV394aXKae3BbWFmyjGAv4EeWllqs=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Magick.NET.Core.wasm",
        "name": "Magick.NET.Core.d2kiqzu43d.wasm",
        "hash": "sha256-FdwPGN0nfZtXydnsotg3UbkTSc3WyZhS5EsPBOMZ62s=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "MicroCom.Runtime.wasm",
        "name": "MicroCom.Runtime.viw2o55mwq.wasm",
        "hash": "sha256-Jy3LZVB8MBkdFkecGnunWSDdyD2xZAIqfIRAd29LhYI=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Microsoft.Bcl.HashCode.wasm",
        "name": "Microsoft.Bcl.HashCode.fhc460s73x.wasm",
        "hash": "sha256-zaHCBrRXAFuOMETGRVpV0g2MyMtc7qajuH+pMRNR2D8=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Microsoft.CSharp.wasm",
        "name": "Microsoft.CSharp.vc74ka0j2f.wasm",
        "hash": "sha256-P7qKvEHMowsS2wvBLOsl84cAPOho5yGcSqn3c4RfAWs=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Microsoft.Extensions.Configuration.Abstractions.wasm",
        "name": "Microsoft.Extensions.Configuration.Abstractions.iqery9nry2.wasm",
        "hash": "sha256-Wtkv6KZJ6c4+tzKrdRIJ2f3EhIakYPDfAn2Rj04zHR0=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Microsoft.Extensions.Configuration.Binder.wasm",
        "name": "Microsoft.Extensions.Configuration.Binder.7ebfchnp6m.wasm",
        "hash": "sha256-bwcA9NPW/7dv3k+Mr4N+gcZdNUYUtuUj3+O8YHwkbMc=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Microsoft.Extensions.Configuration.wasm",
        "name": "Microsoft.Extensions.Configuration.qjwtydrbeh.wasm",
        "hash": "sha256-+o/D7fr2KXPZyZmjiCQLWPHVEh/oiumh8cIcUUlXN4U=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Microsoft.Extensions.DependencyInjection.wasm",
        "name": "Microsoft.Extensions.DependencyInjection.0lh9y2h2sj.wasm",
        "hash": "sha256-RPtn6pkmjQgSJvC2r9Gf9YPVYplQr2s1x/1CuqngAo0=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Microsoft.Extensions.DependencyInjection.Abstractions.wasm",
        "name": "Microsoft.Extensions.DependencyInjection.Abstractions.r4njuum1at.wasm",
        "hash": "sha256-27EFRDR3xbRa0nWlwVoIj7DmFf0h6tMjjYcNWEjPkNM=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Microsoft.Extensions.Logging.Abstractions.wasm",
        "name": "Microsoft.Extensions.Logging.Abstractions.ef2lrhidbn.wasm",
        "hash": "sha256-ij5nuTCdqyB4gAlMGyLXxnmohZYnn3SC233Zqd40tYU=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Microsoft.Extensions.Logging.Configuration.wasm",
        "name": "Microsoft.Extensions.Logging.Configuration.ju4kdlayzb.wasm",
        "hash": "sha256-denqcfC1DRbXUab21lndZ52ZfmVytPHoCyAsnaqbgAI=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Microsoft.Extensions.Logging.Console.wasm",
        "name": "Microsoft.Extensions.Logging.Console.fyawztclpa.wasm",
        "hash": "sha256-iWlCo4G413u4fHtoBAfebyGbGlPw05vGwSdGexzhoiE=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Microsoft.Extensions.Logging.wasm",
        "name": "Microsoft.Extensions.Logging.sceabnebwe.wasm",
        "hash": "sha256-1FHSWAgdUAeGtFFXIYqElWp8gy2mikQNMtTN8+/oo30=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Microsoft.Extensions.Options.ConfigurationExtensions.wasm",
        "name": "Microsoft.Extensions.Options.ConfigurationExtensions.cbvevgdsho.wasm",
        "hash": "sha256-+No81T5HXezhQaZ4c1145qD3lvRa4r9Q3FWXw6ZfgAg=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Microsoft.Extensions.Options.wasm",
        "name": "Microsoft.Extensions.Options.llivjk1whj.wasm",
        "hash": "sha256-6ULG5niYM+MHArol/TOSkMbfxTc7D5jKcorcHt/RPo0=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Microsoft.Extensions.Primitives.wasm",
        "name": "Microsoft.Extensions.Primitives.ndur9bx6x9.wasm",
        "hash": "sha256-ixdbQi4Qj2Bd8xbpEM/764LZNO1roqKfS/hVzZiePNc=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Microsoft.VisualBasic.Core.wasm",
        "name": "Microsoft.VisualBasic.Core.ic4oks67r5.wasm",
        "hash": "sha256-AtC400vi85AY2+3JEyu7M0FZmzy5W/qPTQwnEYgE5WU=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Microsoft.VisualBasic.wasm",
        "name": "Microsoft.VisualBasic.oawltawhmj.wasm",
        "hash": "sha256-QIQIVsgVtOjss3PhGWReXuYJqZfXWjj4Ka9g42s9/kM=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Microsoft.Win32.Primitives.wasm",
        "name": "Microsoft.Win32.Primitives.n6ylnc1rdo.wasm",
        "hash": "sha256-sWj2rJGGR2WaxEbYR4rTRr70SK107Yxg07dskgAB9Gs=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Microsoft.Win32.Registry.wasm",
        "name": "Microsoft.Win32.Registry.205yp9wx9t.wasm",
        "hash": "sha256-LvXRTg75x2rR8pl0Bw85waJhB6rLFe8JUXJZHQVj1AA=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "PeDecoder.wasm",
        "name": "PeDecoder.jbvizdbbsa.wasm",
        "hash": "sha256-8ckwwL8aa0yy1hXumYOYeVLAaSCfy+ttX/4fV4661qs=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "ShimSkiaSharp.wasm",
        "name": "ShimSkiaSharp.b992l1gupc.wasm",
        "hash": "sha256-oQsEwoYeCaCO+icqOOoqTEmEMvdm+u56pKlpabk8hRk=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "SkiaSharp.wasm",
        "name": "SkiaSharp.jplvzyrjai.wasm",
        "hash": "sha256-WUX3pIObMgqBmG3FmMAe+brTVISSmE6jzR95+QNTq3Y=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Svg.Animation.wasm",
        "name": "Svg.Animation.0hhjnh4n9c.wasm",
        "hash": "sha256-NDpd4ECi1jU0qAqBb+/oJs6xtyagAam4FjS8wvBq1wk=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Svg.Controls.Skia.Avalonia.wasm",
        "name": "Svg.Controls.Skia.Avalonia.7cbojbrz9p.wasm",
        "hash": "sha256-Z77jwiA/hidUV0QAQ67nU+3AHccQQ/v3x9rtGUG33dc=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Svg.Custom.wasm",
        "name": "Svg.Custom.xg4m5wsy1n.wasm",
        "hash": "sha256-IaXbt+EU4EIRyQ51SZ6YjA+oSuO3gDUEhda6T3rFPm0=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Svg.Model.wasm",
        "name": "Svg.Model.1q3temtsvr.wasm",
        "hash": "sha256-55HrYHyMDuqVjncEJNWYTsGb2ODMo4x2hgx8Lc974/4=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Svg.SceneGraph.wasm",
        "name": "Svg.SceneGraph.n7z0b0bafo.wasm",
        "hash": "sha256-Q82tWDPWipwOD9ji0yE7tsxgEsCAI59PE2iE9c6XY8g=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "Svg.Skia.wasm",
        "name": "Svg.Skia.eip1g81zhq.wasm",
        "hash": "sha256-mshEzAmNYIzWWXOSontAICkI4PhOrFWd6ExiWhMFX+g=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.AppContext.wasm",
        "name": "System.AppContext.33lgp2bflw.wasm",
        "hash": "sha256-vz/SY7IT6HI0fgNQ8GZWZ+2a6G0TdNW8NiXfBQzCDFg=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Buffers.wasm",
        "name": "System.Buffers.g7kq07we8q.wasm",
        "hash": "sha256-NBuJlJA0h3fyxW5UP4hoYInaEwe9OZxG2gEbjIdmCrY=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Collections.Concurrent.wasm",
        "name": "System.Collections.Concurrent.ayt23dc2he.wasm",
        "hash": "sha256-IntDEoqraHR5cfj4BngQXlQjjlrcOYRCbXuQtRi06AY=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Collections.Immutable.wasm",
        "name": "System.Collections.Immutable.9nsqwhhu4t.wasm",
        "hash": "sha256-haf/04roddsE9PJPtNQ70GK4E5Pgdvs8Xn7GWtwiB8E=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Collections.NonGeneric.wasm",
        "name": "System.Collections.NonGeneric.525tw7pi7d.wasm",
        "hash": "sha256-3Wp37kJjaV1lPseMx3LiVnPOb6jBbwgsqYQ5XifQfcI=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Collections.Specialized.wasm",
        "name": "System.Collections.Specialized.33r4c9ewf6.wasm",
        "hash": "sha256-H5J4UE+YDKwGT/+7eJ04nYWti5iFiGRcy+qc+mQi/O8=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Collections.wasm",
        "name": "System.Collections.gtdrkq93la.wasm",
        "hash": "sha256-TPWn1Sks7F7hYJ6yIoiF5E16QgcubEClSpnYJRxXog8=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.ComponentModel.Annotations.wasm",
        "name": "System.ComponentModel.Annotations.sojjklgz5l.wasm",
        "hash": "sha256-lE1lvR0jcFeSBLguBCsrgQ6E/SwzEWSBo6DT1t+eyzE=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.ComponentModel.DataAnnotations.wasm",
        "name": "System.ComponentModel.DataAnnotations.q8seqyww2r.wasm",
        "hash": "sha256-hlc54d9ROri0ue7K99r10i4zkATiLhcQ9zo0q7W2hR4=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.ComponentModel.EventBasedAsync.wasm",
        "name": "System.ComponentModel.EventBasedAsync.7b6lvtlm4f.wasm",
        "hash": "sha256-M/bTWUvIaq4cjgVjpzDowRr/CILOEX/94pdcYydLVIk=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.ComponentModel.Primitives.wasm",
        "name": "System.ComponentModel.Primitives.nwih63pj9c.wasm",
        "hash": "sha256-93uWz5htHU4mW5eFYLizhbEpyIP7wRw7m5BxQpDfSkA=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.ComponentModel.TypeConverter.wasm",
        "name": "System.ComponentModel.TypeConverter.gq1uo9pwbw.wasm",
        "hash": "sha256-uC84qHy9xx+jXRXst6IaM1xt/w7LxUiXTeriFG4ZAiA=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.ComponentModel.wasm",
        "name": "System.ComponentModel.gtzdp6gjij.wasm",
        "hash": "sha256-lEd4E48MsU5w1IehTBnjkEI2vNCXvuFPvPeMTyw3++U=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Configuration.wasm",
        "name": "System.Configuration.q733dci1qs.wasm",
        "hash": "sha256-BnBG27W6TaVX+iCWFXq00o8F7GIIuUQtO0HkkMmwjMU=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Console.wasm",
        "name": "System.Console.to0mbsovya.wasm",
        "hash": "sha256-AzhPOHmII8SXRU+N5orETTSLHfAesDlXNBoICnlf/hw=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Core.wasm",
        "name": "System.Core.azclj5c5yv.wasm",
        "hash": "sha256-CqRaluF8Fsi/jz9kCwltS3ZZ2H/dItSmY+6I8eMXh+4=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Data.Common.wasm",
        "name": "System.Data.Common.nthckp9a3t.wasm",
        "hash": "sha256-O41AktxbWuV1VI6Jpdpn/SVaBfffTI9wPfARobgjWZ4=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Data.DataSetExtensions.wasm",
        "name": "System.Data.DataSetExtensions.a9dlaxjhfy.wasm",
        "hash": "sha256-ouu5ej3421SgXUzAYf44bxCQ7RUxrfwqwgmEwvh6k6A=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Data.wasm",
        "name": "System.Data.whqnroy0hk.wasm",
        "hash": "sha256-5GA1j3GuS3Q/muwMulPxIEys5V8jEFaVCDIho/UzAZ4=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Diagnostics.Contracts.wasm",
        "name": "System.Diagnostics.Contracts.1ocyilnve1.wasm",
        "hash": "sha256-X5WU0oWl2MmZG7InLJHAf9o6jmX4ihpQpuZh/VxktlM=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Diagnostics.Debug.wasm",
        "name": "System.Diagnostics.Debug.0akkuosl4u.wasm",
        "hash": "sha256-qOE7MrpIyjV9RclZShTpxbW0dXN9NSp6E8O6BCM4j/k=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Diagnostics.DiagnosticSource.wasm",
        "name": "System.Diagnostics.DiagnosticSource.us3v4sw3re.wasm",
        "hash": "sha256-Aqcp7AQ4SF2SidBLhSXGiBMx6wUGEmRGMUGtJuw5Biw=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Diagnostics.FileVersionInfo.wasm",
        "name": "System.Diagnostics.FileVersionInfo.52zf5lzwq0.wasm",
        "hash": "sha256-PShcf3L0QvoHeHX6WO3KQqAuMpM1dUIF3bOHwdvM02k=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Diagnostics.Process.wasm",
        "name": "System.Diagnostics.Process.aksc3z10sf.wasm",
        "hash": "sha256-mnphv8xnpoUBaUm9xj1BKdq1ZtrmXgfNC1pTeUX8Z1A=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Diagnostics.StackTrace.wasm",
        "name": "System.Diagnostics.StackTrace.oyhnn763kn.wasm",
        "hash": "sha256-sM2dU4tKknoFqZJcmyFk/cQpUQVCrmiXLAqIvXTeHB4=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Diagnostics.TextWriterTraceListener.wasm",
        "name": "System.Diagnostics.TextWriterTraceListener.4dqf2t866y.wasm",
        "hash": "sha256-OAmvIaAFxlBfLbdbPMsrpB3c+g8LVN0zOy07o5j5Flw=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Diagnostics.Tools.wasm",
        "name": "System.Diagnostics.Tools.4jmuc696pu.wasm",
        "hash": "sha256-cL64WwiWGdmdH0weZWJRZHKU7pWvR4JGCYHlGJGMroQ=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Diagnostics.TraceSource.wasm",
        "name": "System.Diagnostics.TraceSource.4djwg1x5dk.wasm",
        "hash": "sha256-+NGeFhqlXf6aE9U55il2yFOG92I2uTAw9V9l8WCD8+M=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Diagnostics.Tracing.wasm",
        "name": "System.Diagnostics.Tracing.6xw2h3mehq.wasm",
        "hash": "sha256-KpCxa5K0+bU0W8TPjz8oJwyGdCZpA5ybxcUoEKh1zoA=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Drawing.Primitives.wasm",
        "name": "System.Drawing.Primitives.fht3cwi9ck.wasm",
        "hash": "sha256-/dvJgFicq8TLI4i4VdtMFPN1nKxit3i+vSbzahAe1cs=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Drawing.wasm",
        "name": "System.Drawing.cby5s9a0on.wasm",
        "hash": "sha256-+Dy89WFcA7dUgn0g1UJ8TjdZZQ6V8ZN0kZfrRA8xGJ4=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Dynamic.Runtime.wasm",
        "name": "System.Dynamic.Runtime.yi4mxor0by.wasm",
        "hash": "sha256-lm4ZND3nHWnLXcMzRKXz3I2BGdWPik99ttzib2RVCpw=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Formats.Asn1.wasm",
        "name": "System.Formats.Asn1.56jjdsv3d0.wasm",
        "hash": "sha256-M0vQyUCJLFCrFQX/Bg7bPG8nfXfxjvEYTseZkbfGYxg=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Formats.Tar.wasm",
        "name": "System.Formats.Tar.89bpoq7vrz.wasm",
        "hash": "sha256-xAroer71Mir0OA4S5cPejkZaL03TqTFg1aimzQ9LIok=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Globalization.Calendars.wasm",
        "name": "System.Globalization.Calendars.rildgn7mzz.wasm",
        "hash": "sha256-zZUFwm7oGVSq1qsK8oIZuzSLLXjmcoLgT1iy5qqXHp8=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Globalization.Extensions.wasm",
        "name": "System.Globalization.Extensions.opr37tm2pn.wasm",
        "hash": "sha256-9HWY1aCFc5GoQcysEA0v85vU5i8CRuqZvKqJJpzka6k=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Globalization.wasm",
        "name": "System.Globalization.moqtkrs3kx.wasm",
        "hash": "sha256-qnMi2/9zgAiBieR/7gLp8ZIASS2EbKk5F3FK/ZFHXn8=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.IO.Compression.wasm",
        "name": "System.IO.Compression.6pdq5v22pd.wasm",
        "hash": "sha256-2mhUJF3wl9QgdsvbtOy/ZDArJeyj8crmASfCqaGAtNw=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.IO.Compression.Brotli.wasm",
        "name": "System.IO.Compression.Brotli.7ohel80nih.wasm",
        "hash": "sha256-CZooa2KeN4jmRIpQK41QSkQP6d4dmj++PSm0FlRGkuo=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.IO.Compression.FileSystem.wasm",
        "name": "System.IO.Compression.FileSystem.y83oisb571.wasm",
        "hash": "sha256-clgAvPC5qyl4EnAE8n7NPNbEX9+Y4Rpxpnxl5349y+4=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.IO.Compression.ZipFile.wasm",
        "name": "System.IO.Compression.ZipFile.tmxcz8z1z1.wasm",
        "hash": "sha256-uzfss5y8PFTeOVFAKk+jQLi/FLPKrehaUq6f+1xqeoQ=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.IO.FileSystem.AccessControl.wasm",
        "name": "System.IO.FileSystem.AccessControl.xzbtcseu3i.wasm",
        "hash": "sha256-A0JUayiM8+PP3INOm0TFut8IWKzKDe4PcEZGksJchHU=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.IO.FileSystem.DriveInfo.wasm",
        "name": "System.IO.FileSystem.DriveInfo.g3te4cd6dq.wasm",
        "hash": "sha256-NPFQq5Ilvv6t6twDWNjv6zmCVVMv4GREj5xvBqMG71g=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.IO.FileSystem.Primitives.wasm",
        "name": "System.IO.FileSystem.Primitives.s8z1h6e7bs.wasm",
        "hash": "sha256-rJF209xyy75EKtaVFIXszQG504rNkNMzn5nuQgopOis=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.IO.FileSystem.Watcher.wasm",
        "name": "System.IO.FileSystem.Watcher.jia322rvtg.wasm",
        "hash": "sha256-+1GYGl6rYRxD2mfq9jYmM71NOi5lVQ4ntSd2XgLQr/A=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.IO.FileSystem.wasm",
        "name": "System.IO.FileSystem.y4d3mjm0rs.wasm",
        "hash": "sha256-vjhZDTZZmOeM6kiHpKghpt2WLmy8+s51jgU3nWVoMsQ=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.IO.IsolatedStorage.wasm",
        "name": "System.IO.IsolatedStorage.vucb3c4eed.wasm",
        "hash": "sha256-12gqkUA+EW9Ok+XjNnztHbwcZdF0sHGfYsGFDhkaJHA=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.IO.MemoryMappedFiles.wasm",
        "name": "System.IO.MemoryMappedFiles.slhu4kxyuf.wasm",
        "hash": "sha256-YGcvm9Unqx7Nslxy7M0XGAICX/34Ac4rHJ1GgryesTc=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.IO.Pipelines.wasm",
        "name": "System.IO.Pipelines.d9mh1mhl6a.wasm",
        "hash": "sha256-D2zGD+7FWanAQkGdcxPpzhuAqr1TBe3FwbCzyOEeDDc=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.IO.Pipes.wasm",
        "name": "System.IO.Pipes.6axf1j8g7w.wasm",
        "hash": "sha256-wlyNSMlOoSLtBjH4ahAIRUF/EIraBRKRS1mqdDW1BEc=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.IO.Pipes.AccessControl.wasm",
        "name": "System.IO.Pipes.AccessControl.aawhalzgje.wasm",
        "hash": "sha256-sqt6fPu+tFpCJkBRmzTwCeFxWuNkv9Qvw6RRdtyLi/Y=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.IO.UnmanagedMemoryStream.wasm",
        "name": "System.IO.UnmanagedMemoryStream.jtjypz1bw3.wasm",
        "hash": "sha256-141FexMM+rgyJjbPV3Tqjins7IWx6QuAhdpWiGB8+wU=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.IO.wasm",
        "name": "System.IO.u03kcd51u2.wasm",
        "hash": "sha256-TrxbGqQLHCo+lECXU34H+9JxDj+EGVrjMrSa4JIhTpQ=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Linq.AsyncEnumerable.wasm",
        "name": "System.Linq.AsyncEnumerable.qecnowng39.wasm",
        "hash": "sha256-kqYBPZyPoXxShJfn6Z4etPhqhi7wXEdqUL+YvjNrMa0=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Linq.Expressions.wasm",
        "name": "System.Linq.Expressions.48ueoty928.wasm",
        "hash": "sha256-hJy4f+HmGU4Xh4kza5OapM/UfFxlTnF0arbzmWUqEPM=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Linq.Parallel.wasm",
        "name": "System.Linq.Parallel.6f9mb35gje.wasm",
        "hash": "sha256-MgAUM4mdUmMKSpa9kowpy8pfI5inqZyMsDmFDZYeKFU=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Linq.Queryable.wasm",
        "name": "System.Linq.Queryable.bv9ee87bon.wasm",
        "hash": "sha256-d0dArG+pt8tZXMUGpFhpI18ztIMnH4waXkZ6KsKhqEI=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Linq.wasm",
        "name": "System.Linq.ffyao6dtjo.wasm",
        "hash": "sha256-y8xCXRiQfpwTV3uVD+XU9IZyYOBV3IUhbEzI8/FlECQ=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Memory.wasm",
        "name": "System.Memory.i6uetwkqik.wasm",
        "hash": "sha256-Sg0IsbGxUOdZ+8GEG2rO5+mJutwVXDv/c3DWvC0Y1LM=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Net.Http.wasm",
        "name": "System.Net.Http.9odm4o2yfw.wasm",
        "hash": "sha256-R42oDv3s94KCE4lozGAQ19Wa5RpsPf0UyE2KXiTvME8=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Net.Http.Json.wasm",
        "name": "System.Net.Http.Json.lkg2pjap28.wasm",
        "hash": "sha256-ZYX5QZNYs9FBGbSqFOEHB1CSxj2o3HZlYgB+H8dfls8=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Net.HttpListener.wasm",
        "name": "System.Net.HttpListener.l269ngb85c.wasm",
        "hash": "sha256-/e2K/6DbHfR5WYWSWPEyXxwbwdeGUtCBfIxY3PyKhaI=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Net.Mail.wasm",
        "name": "System.Net.Mail.7s5j7d2u11.wasm",
        "hash": "sha256-B/+5emPbFkpK82pnJSnAmTzRuY7KRKi7wvTfFnULfh4=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Net.NameResolution.wasm",
        "name": "System.Net.NameResolution.y3a25frvkb.wasm",
        "hash": "sha256-Ui9OtscoCa7Rn+R6uggcz2M7ywzHL64xb5x7hqyrRl8=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Net.NetworkInformation.wasm",
        "name": "System.Net.NetworkInformation.6hh4xfj68d.wasm",
        "hash": "sha256-ej7w2CMrRXME9AFCOPRuLwPTCNJ5MfZYGaGYL4xpwiU=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Net.Ping.wasm",
        "name": "System.Net.Ping.blplcg5q7j.wasm",
        "hash": "sha256-Me8xKvJFcpGmQz0AsV1c2r9vpfCIAUEvRS8wXBIlB8s=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Net.Primitives.wasm",
        "name": "System.Net.Primitives.52l52f7ge8.wasm",
        "hash": "sha256-I7jNDlc6aY2jmzdqNKjL/MZtDTu9OXjmsPD4nXyeFio=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Net.Quic.wasm",
        "name": "System.Net.Quic.aw952wuprs.wasm",
        "hash": "sha256-WqNOpml+SO8zGljApjO78PEvcIFKqDWK0LVTSKsFxIA=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Net.Requests.wasm",
        "name": "System.Net.Requests.k1mlkdyp0u.wasm",
        "hash": "sha256-KFXJT6HgrkCQDdxsNE6gg+fQHpN4MfAkgt3h6YXYvbM=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Net.Security.wasm",
        "name": "System.Net.Security.vm6w2umju9.wasm",
        "hash": "sha256-l4OlxqQwu+sYhjLriuJSuMwHNJ+r/BGVrXp92H+S3NQ=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Net.ServerSentEvents.wasm",
        "name": "System.Net.ServerSentEvents.adh3kntjlv.wasm",
        "hash": "sha256-rlq89aqpXUJXWQVg3RSoAJlvCpD0wwttk75XFhW7Bps=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Net.ServicePoint.wasm",
        "name": "System.Net.ServicePoint.v8e4bui0ea.wasm",
        "hash": "sha256-4Zv++S2GQBCLLKZxZWQfRW3vgfFi9iddABhfC0/YyoM=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Net.Sockets.wasm",
        "name": "System.Net.Sockets.ts6iauk88c.wasm",
        "hash": "sha256-E0uMfyK7c1v52Ax4PT7Al8rOKXtImY4piCmVV1h36sA=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Net.WebClient.wasm",
        "name": "System.Net.WebClient.od0uvbnjit.wasm",
        "hash": "sha256-bBhkM6WRjt9KHqN/yR59aCMpgcg5lxrlHszjVilE9h0=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Net.WebHeaderCollection.wasm",
        "name": "System.Net.WebHeaderCollection.qf50eeagye.wasm",
        "hash": "sha256-hoFgNcO9TCczLQwT6qI84eb3J3qaSt95k4Jk1T6d4G8=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Net.WebProxy.wasm",
        "name": "System.Net.WebProxy.iavyox40gr.wasm",
        "hash": "sha256-6k7n9kRTIjsvj7OlfcaAt7JtbA1BYzx/krXVj9B2al8=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Net.WebSockets.Client.wasm",
        "name": "System.Net.WebSockets.Client.qeq362432l.wasm",
        "hash": "sha256-jknBfKz+BaTegYBkGof7b0J6y4A2yMaaO9A7SYAq0XY=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Net.WebSockets.wasm",
        "name": "System.Net.WebSockets.oek2p1up7o.wasm",
        "hash": "sha256-MK9DXHHm6AKhF5Je0S8xPCHgY0e0z4i+GMJ6TMhNOoI=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Net.wasm",
        "name": "System.Net.i47gzsagt6.wasm",
        "hash": "sha256-7uRJ8dcmw3n7qTbvh1xPYnY0vmPYLapuPLmNF0k4e8I=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Numerics.Vectors.wasm",
        "name": "System.Numerics.Vectors.n63j92lpq0.wasm",
        "hash": "sha256-3xe0W+JmwQNNagoBuTG5UEUhgmotX8PmdKsPci5JnPA=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Numerics.wasm",
        "name": "System.Numerics.rkirn4ewf6.wasm",
        "hash": "sha256-y32dbXS60w1HfL97xSaaoig78fCA2hQirSUfe9ZGl30=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.ObjectModel.wasm",
        "name": "System.ObjectModel.i378yy84uu.wasm",
        "hash": "sha256-EoCxZODvWTOXgxLrSIGtMa2yjMKLeeK1JlfMuIAfMbc=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Private.DataContractSerialization.wasm",
        "name": "System.Private.DataContractSerialization.sny9oej0eg.wasm",
        "hash": "sha256-SKiQcTZvy/3E4yYZXbAuxojifAxqCAALhFducmouT7M=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Private.Uri.wasm",
        "name": "System.Private.Uri.esflyl9tnu.wasm",
        "hash": "sha256-LunJHkRu5RgV5fHjx5eDdT17J4Gtytr6/oKJwO1d8R0=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Private.Xml.Linq.wasm",
        "name": "System.Private.Xml.Linq.memmbl9fkq.wasm",
        "hash": "sha256-EjZ9rzHEnY68PXt05q3B/kbsekIDstXc7Sn5XRDvk1E=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Private.Xml.wasm",
        "name": "System.Private.Xml.zw12n4yipy.wasm",
        "hash": "sha256-Mx0L0SdPw6FcjUMwHIOX5eE8YOVQ6jMFlrmRiK+KkhQ=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Reflection.wasm",
        "name": "System.Reflection.14urzokzzb.wasm",
        "hash": "sha256-z8vs8oc+dKapwTQeJ57oDBrsDsHLYwVv3iVtzebNxZo=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Reflection.DispatchProxy.wasm",
        "name": "System.Reflection.DispatchProxy.3vrt3hlgt3.wasm",
        "hash": "sha256-T0nAzwQ4akMO9XJb2cgIhZsnc78p2+fxtnVD40l1XA4=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Reflection.Emit.ILGeneration.wasm",
        "name": "System.Reflection.Emit.ILGeneration.ng7j4cz770.wasm",
        "hash": "sha256-eS8SCSrXQiex2kgp1qxb8IRQqdqTozoZXpt+CoM5v1U=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Reflection.Emit.Lightweight.wasm",
        "name": "System.Reflection.Emit.Lightweight.v2imlilbpe.wasm",
        "hash": "sha256-B9LTebLgF9ZAw2P8ct6U1bZ5++SFfNBBam+PO4a4ELo=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Reflection.Emit.wasm",
        "name": "System.Reflection.Emit.udsnuk3ij6.wasm",
        "hash": "sha256-jrKQzTgqMdjuqWHUGsPfnCbqnWpXQkvKfDfBSM+Mt0g=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Reflection.Extensions.wasm",
        "name": "System.Reflection.Extensions.182qm6arqh.wasm",
        "hash": "sha256-P2Qk2OLacQqt2Llp1r47hyo48QLV06MpAlAZXa35u8c=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Reflection.Metadata.wasm",
        "name": "System.Reflection.Metadata.5mrm43mmwy.wasm",
        "hash": "sha256-TYUTGCId0GxyLd2Jv9MzjCaGIiyVHaCQ9dFDARceTQA=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Reflection.Primitives.wasm",
        "name": "System.Reflection.Primitives.2qdwh2ddjb.wasm",
        "hash": "sha256-ehYMFAYAGb49ZR5SbH909AXh3SHOi+60MV9aTsA4eNc=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Reflection.TypeExtensions.wasm",
        "name": "System.Reflection.TypeExtensions.ohzfghpj7u.wasm",
        "hash": "sha256-bKXQADwSKixye7MZzqGBpXSYCeCyp5ReO68y4cVZsnE=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Resources.Reader.wasm",
        "name": "System.Resources.Reader.o5l4jqwkcn.wasm",
        "hash": "sha256-HN9FyXSEtGpwrjaWYqr0BYnAF5LhvVg5YyAOfj59K/w=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Resources.ResourceManager.wasm",
        "name": "System.Resources.ResourceManager.u0kj92um25.wasm",
        "hash": "sha256-4sISLJ4CC3eWntk5TFLGQP39skJD43fZt2tHEKDrQ7w=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Resources.Writer.wasm",
        "name": "System.Resources.Writer.fybhus075b.wasm",
        "hash": "sha256-x+TbpI+502ck5N3QVtiTNFvaRmEzfM6AUQezqym5L34=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Runtime.CompilerServices.Unsafe.wasm",
        "name": "System.Runtime.CompilerServices.Unsafe.a2v8uztr6q.wasm",
        "hash": "sha256-vtZgLdKVZ4XAh9LNuAg07pdJWav2ZTSdji3wBl6Tnak=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Runtime.CompilerServices.VisualC.wasm",
        "name": "System.Runtime.CompilerServices.VisualC.h9ibm7eot2.wasm",
        "hash": "sha256-NWO0niQAT2U38QP2p678YrYceWxfcGZiao7o4SRm/lI=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Runtime.Extensions.wasm",
        "name": "System.Runtime.Extensions.oueezzziy7.wasm",
        "hash": "sha256-sGE96dH2hSZ2o95KQyX/KhuQGnsAFEHdXfHf2rJrSYY=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Runtime.Handles.wasm",
        "name": "System.Runtime.Handles.pegxo55rww.wasm",
        "hash": "sha256-r9JfKdsE92m/EaS+G9TnZ8alLJ37y9loMJDwpuhyt/4=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Runtime.InteropServices.wasm",
        "name": "System.Runtime.InteropServices.76dk94nxht.wasm",
        "hash": "sha256-r2gFCA+pgFEb2L9UVD2ikpc1TLVxMjWdkQeuSoAnsdk=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Runtime.InteropServices.RuntimeInformation.wasm",
        "name": "System.Runtime.InteropServices.RuntimeInformation.himsthc3a6.wasm",
        "hash": "sha256-+QFSX4Pa6oEemBGAbLSx6ddeliBDgepZ+wpUyQqPbsw=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Runtime.Intrinsics.wasm",
        "name": "System.Runtime.Intrinsics.pxgyd8cnpl.wasm",
        "hash": "sha256-w6hLmBdElU3LjlQ9rQqwli8s0kgF20g9OJ+9AbS+AZw=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Runtime.Loader.wasm",
        "name": "System.Runtime.Loader.smrevcdjv4.wasm",
        "hash": "sha256-nL05yWh/sEH+UGtqYaYsoxnQbkeZ2wfZIJdSlMdUVuU=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Runtime.Numerics.wasm",
        "name": "System.Runtime.Numerics.c79r80p0xa.wasm",
        "hash": "sha256-qAA+g2R9BL+KkiVw/uSdgBzcMyh6lHjvE8btAPFlf44=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Runtime.Serialization.Formatters.wasm",
        "name": "System.Runtime.Serialization.Formatters.5nvzlh22y8.wasm",
        "hash": "sha256-8UOlvjQNeuJCJVs/80yedxgANKXEv+u783ZjeVX9lxA=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Runtime.Serialization.Json.wasm",
        "name": "System.Runtime.Serialization.Json.4u1byp8ddu.wasm",
        "hash": "sha256-9PvflPMpTgXt/CJr9aPaMFxrcfb9dfyCWfQcHjBZ/Lw=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Runtime.Serialization.Primitives.wasm",
        "name": "System.Runtime.Serialization.Primitives.0677mizgg8.wasm",
        "hash": "sha256-CK47ivfrIuMw7yupuqFV24kdTeMPxzka4edYB/BUK9I=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Runtime.Serialization.Xml.wasm",
        "name": "System.Runtime.Serialization.Xml.7t5vv8ymb3.wasm",
        "hash": "sha256-1ZF5POZglEr7ftw2T6suEvJDtXBOaTWjwiFNqHM6A14=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Runtime.Serialization.wasm",
        "name": "System.Runtime.Serialization.xyb15dfsu9.wasm",
        "hash": "sha256-2VvOlAvLHV9Hmwglfi2ZMv/OBChHICovaG0z7JoV4/s=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Runtime.wasm",
        "name": "System.Runtime.p5wq7tenlq.wasm",
        "hash": "sha256-sz8dIFsswef1k1Xw7c0y0c+hKmO4ytyw9WoY17i+cAU=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Security.AccessControl.wasm",
        "name": "System.Security.AccessControl.9gsd8k6c5m.wasm",
        "hash": "sha256-t/Zhd+N29fKKAil705KO0KB8ytqtRMQ2gIUgle8vQxU=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Security.Claims.wasm",
        "name": "System.Security.Claims.l0ljqfaxwo.wasm",
        "hash": "sha256-JcfUg2gv1aAcLpXhGHpawjy/Te3Qg9fcheHexSFJsec=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Security.Cryptography.wasm",
        "name": "System.Security.Cryptography.2ufhqnonzh.wasm",
        "hash": "sha256-asDjhVEPO/QN5eJuPJXMjymopyT1voLEIPbRMo6SpJM=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Security.Cryptography.Algorithms.wasm",
        "name": "System.Security.Cryptography.Algorithms.i0xv6lwo7z.wasm",
        "hash": "sha256-4vLqnvj48uhFTeNavhXSPLej8+a04vT4Q4BzeA7aOJc=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Security.Cryptography.Cng.wasm",
        "name": "System.Security.Cryptography.Cng.eeehrv0scy.wasm",
        "hash": "sha256-2sUCiaSAJyXHG/+fD4xucBGmWPzXRTVKmMn/tpyrwMs=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Security.Cryptography.Csp.wasm",
        "name": "System.Security.Cryptography.Csp.2c9gox8ebd.wasm",
        "hash": "sha256-vjBPq4f54O+P5zvokIBJUwXcSpez9Y+PGc0wtzqW6W4=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Security.Cryptography.Encoding.wasm",
        "name": "System.Security.Cryptography.Encoding.axny8s8hqs.wasm",
        "hash": "sha256-Ypo7uNzxzTGJIC9jT/m/b4kN4txarBQHeb5H9RXu54k=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Security.Cryptography.OpenSsl.wasm",
        "name": "System.Security.Cryptography.OpenSsl.1sxm4ayhhy.wasm",
        "hash": "sha256-gaDsyTJrtp4XzQYUXgmfB6RLIGi2FjWj8Ar3P9WDT/4=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Security.Cryptography.Primitives.wasm",
        "name": "System.Security.Cryptography.Primitives.4ri0a9h1v4.wasm",
        "hash": "sha256-8LydFSbE7V5LdD50fXdpmeIt/m2sksGJqzMY5e6vP4Y=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Security.Cryptography.X509Certificates.wasm",
        "name": "System.Security.Cryptography.X509Certificates.rncai5fzz3.wasm",
        "hash": "sha256-aWvCLknwVe+mKmgXO2xDc5IhC7B6PLDmWOFFf933b/o=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Security.Principal.wasm",
        "name": "System.Security.Principal.1tjw5xr7f1.wasm",
        "hash": "sha256-ReXTuJnKsfUr67WZ7ekLkKwdCADP7MLsv5+8Rz9kUbM=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Security.Principal.Windows.wasm",
        "name": "System.Security.Principal.Windows.9gaajfy1mm.wasm",
        "hash": "sha256-aUJqbqjVpUgPNm/I10ZI5tTZvdAHZuXZHM1+bgtEh2Q=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Security.SecureString.wasm",
        "name": "System.Security.SecureString.hbgjudeeg7.wasm",
        "hash": "sha256-XWsfTgbMvq8bIErWG9SRS12GjUA70gfii8Zn1x06wo4=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Security.wasm",
        "name": "System.Security.ua0hvadql2.wasm",
        "hash": "sha256-xlJkQOl9i3U3R0rHBnv42WUyxpyTfb4NWeQP9thvn8k=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.ServiceModel.Web.wasm",
        "name": "System.ServiceModel.Web.a5nn12k2du.wasm",
        "hash": "sha256-EoKhlu6LFf/rxXLNN0AG/ox3cVZSQZcoJHRMn3Ai3ZM=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.ServiceProcess.wasm",
        "name": "System.ServiceProcess.mzhiaa0o9m.wasm",
        "hash": "sha256-kt3OXJkHVIZ0HaKa/O5mKV/gKK2VRUPUcZlR1mMEWqI=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Text.Encoding.CodePages.wasm",
        "name": "System.Text.Encoding.CodePages.fso8gniga6.wasm",
        "hash": "sha256-fxt6zzGzOdxcGfVxV+shFrqbQSKxSfGJNB7/GqvLsB8=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Text.Encoding.Extensions.wasm",
        "name": "System.Text.Encoding.Extensions.jth4fv0ujc.wasm",
        "hash": "sha256-N/7bmYLlo54Cs+Dw8ee7CkFrLzqTe8izc5vm6dvzizM=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Text.Encoding.wasm",
        "name": "System.Text.Encoding.hen1lijr2s.wasm",
        "hash": "sha256-RybkW+87HvDCrKwpsSLXM3w5TB5EMz7dpLJz1o+fFaI=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Text.Encodings.Web.wasm",
        "name": "System.Text.Encodings.Web.s63xwzfs6e.wasm",
        "hash": "sha256-ZAoPykd2oHgSQyIJbM0ktKOTvV6mh7f8h4Ev8xRgPJk=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Text.Json.wasm",
        "name": "System.Text.Json.e68ojdl57q.wasm",
        "hash": "sha256-ZhbF4R3Uf6o3EptGsHICrb2qcZ2bd1UEGfHtMc+VkxY=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Text.RegularExpressions.wasm",
        "name": "System.Text.RegularExpressions.o5g45g5kfe.wasm",
        "hash": "sha256-zN+88osSOmhdIdVgihibx/x1XedKWsCYEgvtcYvxypY=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Threading.AccessControl.wasm",
        "name": "System.Threading.AccessControl.vdpdtl3x6u.wasm",
        "hash": "sha256-w+LhmzUpdAZw6jVH9FdmsnN/n9z3LtLHTZR6pyxGSzg=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Threading.Channels.wasm",
        "name": "System.Threading.Channels.wty3mw4oue.wasm",
        "hash": "sha256-7JYMWtx5aaztvI6qsJStob8sRMLyMrT6oDNkD24Inaw=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Threading.Overlapped.wasm",
        "name": "System.Threading.Overlapped.03j0f78xbg.wasm",
        "hash": "sha256-nAsVVeqQDNVR0DaIhT//1tZcxyCxt6LUsnXjhz9mevo=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Threading.Tasks.Dataflow.wasm",
        "name": "System.Threading.Tasks.Dataflow.1qphddued4.wasm",
        "hash": "sha256-h4ITo++VVySJB9UXW1AizkqXGbRc7hysEJPM8paCQBQ=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Threading.Tasks.Extensions.wasm",
        "name": "System.Threading.Tasks.Extensions.a6m6fd88nw.wasm",
        "hash": "sha256-Pgd/40O6xPGNbTDdtZ7IXhjog5u+uiSnSeblax0Kd2w=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Threading.Tasks.Parallel.wasm",
        "name": "System.Threading.Tasks.Parallel.hwicu9gnb7.wasm",
        "hash": "sha256-sUxZoPUOtigPLyVhliXgQ79G8wxA8VXhlK86oWh8RhU=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Threading.Tasks.wasm",
        "name": "System.Threading.Tasks.ko7yntasn6.wasm",
        "hash": "sha256-ZAZ/8IqOgKMUtbsFNyZ/nGPxSrgnh8t4h5Z+SVjecyY=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Threading.Thread.wasm",
        "name": "System.Threading.Thread.ydz2qayyqe.wasm",
        "hash": "sha256-poEtzjS5NNRnotBgaKA2Su+mjp8Y0eXTf7yoYasFvAQ=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Threading.ThreadPool.wasm",
        "name": "System.Threading.ThreadPool.4f7y4725hj.wasm",
        "hash": "sha256-ELorBXMa++9uTr6P3crPiAaJMWcEhNok01GvE1QPjjs=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Threading.Timer.wasm",
        "name": "System.Threading.Timer.23ma43391f.wasm",
        "hash": "sha256-spGH+vGozMmLNWcgCowJHX/L9fBEdoHOEveQK9OdmxI=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Threading.wasm",
        "name": "System.Threading.xg0tuqpte9.wasm",
        "hash": "sha256-XwK+Pb+6p1iYXhc2Ks+FjDwPBgIInT2jAWOQ1jEQsNM=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Transactions.wasm",
        "name": "System.Transactions.1k679dq1nh.wasm",
        "hash": "sha256-DwZ5kQKS8S//BhRIzWNpCBtH7PWn1RUGmLKfunzItyI=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Transactions.Local.wasm",
        "name": "System.Transactions.Local.vhgy89wfp9.wasm",
        "hash": "sha256-/etlP37Z3fbLkp3G+SuI8t2EuBlW6ArNkc821ii6YrU=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.ValueTuple.wasm",
        "name": "System.ValueTuple.gm53bht7l7.wasm",
        "hash": "sha256-yGHllPKjSV7O8dufUyuOGmky/GbwzH0IlPDpgsfuZG4=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Web.HttpUtility.wasm",
        "name": "System.Web.HttpUtility.xg7fizhwzt.wasm",
        "hash": "sha256-b150ZISvFUH8voYwQdhIH/m8vkHcJFY+LFDfuemZ2sw=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Web.wasm",
        "name": "System.Web.sakoricpi1.wasm",
        "hash": "sha256-PADclgEUTNPtXvaKE9YgE1DRcW/Q+gPLdwMUsbu8esA=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Windows.wasm",
        "name": "System.Windows.98z5iu2ekq.wasm",
        "hash": "sha256-mWvSjccrBX09Q7oIJzRGJIi6zQl0pJMKkSrWN9kWf6Y=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Xml.Linq.wasm",
        "name": "System.Xml.Linq.4w1i83kr6d.wasm",
        "hash": "sha256-FM7him4hRwYj3b/rUlLVoDuzrDlojL6J1pMTiiv+prk=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Xml.ReaderWriter.wasm",
        "name": "System.Xml.ReaderWriter.gt2wkfrpd2.wasm",
        "hash": "sha256-RPbWKMyF0PcAToP99SZP3iI1LbK+2mMn0hPGIrJuwMI=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Xml.Serialization.wasm",
        "name": "System.Xml.Serialization.ymsdqvtcj5.wasm",
        "hash": "sha256-OtgEHFA+kC4Og+9sy9AItDeN0G36r5KKP80acvekmKQ=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Xml.XDocument.wasm",
        "name": "System.Xml.XDocument.nthxjrhml8.wasm",
        "hash": "sha256-hVjT4ogBaijnAej8Jkvvfi/3R1Sz65EOhwcQavaAH8I=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Xml.XPath.XDocument.wasm",
        "name": "System.Xml.XPath.XDocument.64udcjx5rc.wasm",
        "hash": "sha256-Sh41QbA3ENHi4CfpbioGcvLy8HrcPA2WVz/gL9fH5lA=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Xml.XPath.wasm",
        "name": "System.Xml.XPath.m6ukudree9.wasm",
        "hash": "sha256-zjcKkw6BNuo0vSx0rlxPlbNuoXjUPWYT9Q1xX548wAU=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Xml.XmlDocument.wasm",
        "name": "System.Xml.XmlDocument.uqap183j3j.wasm",
        "hash": "sha256-WkzkDq1p5Hr8baeIcyUvZkqCz0Qtl39PQvxQVm0EQkA=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Xml.XmlSerializer.wasm",
        "name": "System.Xml.XmlSerializer.kyxgnik2yp.wasm",
        "hash": "sha256-7M5Yf1qrKhJZ1ts2o6P73IDZwOinWT1m/rX9xwhvX18=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.Xml.wasm",
        "name": "System.Xml.gccv71ojjn.wasm",
        "hash": "sha256-QJsL5vasxIwvmlNTlOtLvB4u9y1eFnTzQk9qyIkunDo=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "System.wasm",
        "name": "System.q2zv1ajo7g.wasm",
        "hash": "sha256-UvzF6QRtJEhekaFpn7/fdSo6I77TIVOQYu+B0IvKLAw=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "WindowsBase.wasm",
        "name": "WindowsBase.nd3r6urbkm.wasm",
        "hash": "sha256-JUj0SUenc5aNsprOIJomnkeHQU0zOM9KnaF9DBYp5as=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "mscorlib.wasm",
        "name": "mscorlib.l31p083pc1.wasm",
        "hash": "sha256-L55XGcSlCWjg/nPH8pqX4FoLUqcw89+V7xgu0GWwKSU=",
        "cache": "force-cache"
      },
      {
        "virtualPath": "netstandard.wasm",
        "name": "netstandard.8ql06rx5ae.wasm",
        "hash": "sha256-ANBTDvID1f1UUcJOBmDfE5tFVLna1UUVAxMEbtfGiRM=",
        "cache": "force-cache"
      }
    ]
  },
  "debugLevel": 0,
  "globalizationMode": "sharded",
  "runtimeConfig": {
    "runtimeOptions": {
      "configProperties": {
        "MVVMTOOLKIT_ENABLE_INOTIFYPROPERTYCHANGING_SUPPORT": true,
        "System.Diagnostics.Debugger.IsSupported": false,
        "System.Diagnostics.Metrics.Meter.IsSupported": false,
        "System.Diagnostics.Tracing.EventSource.IsSupported": false,
        "System.Globalization.Invariant": false,
        "System.TimeZoneInfo.Invariant": false,
        "System.Linq.Enumerable.IsSizeOptimized": true,
        "System.Net.Http.EnableActivityPropagation": false,
        "System.Net.Http.WasmEnableStreamingResponse": true,
        "System.Net.SocketsHttpHandler.Http3Support": false,
        "System.Reflection.Metadata.MetadataUpdater.IsSupported": false,
        "System.Resources.UseSystemResourceKeys": true,
        "System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization": false,
        "System.Text.Encoding.EnableUnsafeUTF7Encoding": false
      }
    }
  }
}/*json-end*/);export{gt as default,ft as dotnet,mt as exit};
