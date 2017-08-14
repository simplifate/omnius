document.write('<' + 'script src="https://connect.trezor.io/2/connect.js"></' + 'script>');

var TT = {
  init: function() {
    $('#btnSign').click(this.sign);
  },
  
  sign: function() {
    var ext1 = 'xpub6CgTMowwbfxGb6hFqv5Cw4eyNstNsPeTkj4ofTGpx4PZLVSiiL9dSGngtwgPim563YqyB2dicH7B8Y9GfY1E5ahHVwCcARatkS3RgEobTcy';
    var ext2 = 'xpub6CKyyhypaF9SCfZkHGrYJFV5a2bwE5tpsM89GgMbqnJb7c9NnG77YGmmtjnGvuTNS5F2TdeK3WSdZoGmX96KpqpVu2Mkay2bcNXArN8DgN8';
    
    // spend a multisig input
    var inputs = [{
      address_n: [1],
      prev_index: 0,
      prev_hash: 'd1d08ea63255af4ad16b098e9885a252632086fa6be53301521d05253ce8a73d',
      script_type: 'SPENDMULTISIG',
      multisig: {
        pubkeys: [{node: ext1, address_n: [1]},
                  {node: ext2, address_n: [1]}],
        signatures: ['', ''],
        m: 2
      }
    }];
    // send to PAYTOADDRESS output and a change output
    var outputs = [{
      script_type: 'PAYTOADDRESS',
      amount: 1000,
      address: '1C6hqftsnPZUdjAaEDY6H1XmNetg9HtxNM'
    }, {
      script_type: 'PAYTOMULTISIG',
      amount: 1000,
      multisig: {
        pubkeys: [{node: ext1, address_n: [2]},
                  {node: ext2, address_n: [2]}],
        signatures: ['', ''],
        m: 2
      }
    }];
    TrezorConnect.signTx(inputs, outputs, function (response) {
      if (response.success) {
        console.log('Serialized TX:', response.serialized_tx); // tx in hex
        console.log('Signatures:', response.signatures); // array of signatures, in hex
      } else {
        console.error('Error:', response.error); // error message
      }
      document.getElementById("response").innerHTML = JSON.stringify(response, undefined, 2);
    });
  }
};

$($.proxy(TT.init, TT));