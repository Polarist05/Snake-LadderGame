mergeInto(LibraryManager.library, {

  ShowMessage: function (str) {
      var convertedText = UTF8ToString(str);
      receiveMessageFromUnity(convertedText);
  },
  OpenSummarize: function (str){
    var code = UTF8ToString(str);
    Summarize(code);
  }

});