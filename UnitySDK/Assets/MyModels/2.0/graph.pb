node {
  name: "inp"
  op: "Placeholder"
  attr {
    key: "dtype"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "shape"
    value {
      shape {
        dim {
          size: -1
        }
        dim {
          size: 784
        }
      }
    }
  }
}
node {
  name: "shape"
  op: "Placeholder"
  attr {
    key: "dtype"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "shape"
    value {
      shape {
        dim {
          size: -1
        }
        dim {
          size: 2
        }
      }
    }
  }
}
node {
  name: "zeros/shape_as_tensor"
  op: "Const"
  attr {
    key: "dtype"
    value {
      type: DT_INT32
    }
  }
  attr {
    key: "value"
    value {
      tensor {
        dtype: DT_INT32
        tensor_shape {
          dim {
            size: 2
          }
        }
        tensor_content: "\020\003\000\000\002\000\000\000"
      }
    }
  }
}
node {
  name: "zeros/Const"
  op: "Const"
  attr {
    key: "dtype"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "value"
    value {
      tensor {
        dtype: DT_FLOAT
        tensor_shape {
        }
        float_val: 0.0
      }
    }
  }
}
node {
  name: "zeros"
  op: "Fill"
  input: "zeros/shape_as_tensor"
  input: "zeros/Const"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "index_type"
    value {
      type: DT_INT32
    }
  }
}
node {
  name: "weights"
  op: "VariableV2"
  attr {
    key: "container"
    value {
      s: ""
    }
  }
  attr {
    key: "dtype"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "shape"
    value {
      shape {
        dim {
          size: 784
        }
        dim {
          size: 2
        }
      }
    }
  }
  attr {
    key: "shared_name"
    value {
      s: ""
    }
  }
}
node {
  name: "weights/Assign"
  op: "Assign"
  input: "weights"
  input: "zeros"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "_class"
    value {
      list {
        s: "loc:@weights"
      }
    }
  }
  attr {
    key: "use_locking"
    value {
      b: true
    }
  }
  attr {
    key: "validate_shape"
    value {
      b: true
    }
  }
}
node {
  name: "weights/read"
  op: "Identity"
  input: "weights"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "_class"
    value {
      list {
        s: "loc:@weights"
      }
    }
  }
}
node {
  name: "zeros_1/shape_as_tensor"
  op: "Const"
  attr {
    key: "dtype"
    value {
      type: DT_INT32
    }
  }
  attr {
    key: "value"
    value {
      tensor {
        dtype: DT_INT32
        tensor_shape {
          dim {
            size: 1
          }
        }
        int_val: 2
      }
    }
  }
}
node {
  name: "zeros_1/Const"
  op: "Const"
  attr {
    key: "dtype"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "value"
    value {
      tensor {
        dtype: DT_FLOAT
        tensor_shape {
        }
        float_val: 0.0
      }
    }
  }
}
node {
  name: "zeros_1"
  op: "Fill"
  input: "zeros_1/shape_as_tensor"
  input: "zeros_1/Const"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "index_type"
    value {
      type: DT_INT32
    }
  }
}
node {
  name: "Variable"
  op: "VariableV2"
  attr {
    key: "container"
    value {
      s: ""
    }
  }
  attr {
    key: "dtype"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "shape"
    value {
      shape {
        dim {
          size: 2
        }
      }
    }
  }
  attr {
    key: "shared_name"
    value {
      s: ""
    }
  }
}
node {
  name: "Variable/Assign"
  op: "Assign"
  input: "Variable"
  input: "zeros_1"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "_class"
    value {
      list {
        s: "loc:@Variable"
      }
    }
  }
  attr {
    key: "use_locking"
    value {
      b: true
    }
  }
  attr {
    key: "validate_shape"
    value {
      b: true
    }
  }
}
node {
  name: "Variable/read"
  op: "Identity"
  input: "Variable"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "_class"
    value {
      list {
        s: "loc:@Variable"
      }
    }
  }
}
node {
  name: "Wx_b/MatMul"
  op: "MatMul"
  input: "inp"
  input: "weights/read"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "transpose_a"
    value {
      b: false
    }
  }
  attr {
    key: "transpose_b"
    value {
      b: false
    }
  }
}
node {
  name: "Wx_b/add"
  op: "Add"
  input: "Wx_b/MatMul"
  input: "Variable/read"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
}
node {
  name: "Wx_b/output_node"
  op: "Softmax"
  input: "Wx_b/add"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
}
node {
  name: "weights_1/tag"
  op: "Const"
  attr {
    key: "dtype"
    value {
      type: DT_STRING
    }
  }
  attr {
    key: "value"
    value {
      tensor {
        dtype: DT_STRING
        tensor_shape {
        }
        string_val: "weights_1"
      }
    }
  }
}
node {
  name: "weights_1"
  op: "HistogramSummary"
  input: "weights_1/tag"
  input: "weights/read"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
}
node {
  name: "biases/tag"
  op: "Const"
  attr {
    key: "dtype"
    value {
      type: DT_STRING
    }
  }
  attr {
    key: "value"
    value {
      tensor {
        dtype: DT_STRING
        tensor_shape {
        }
        string_val: "biases"
      }
    }
  }
}
node {
  name: "biases"
  op: "HistogramSummary"
  input: "biases/tag"
  input: "Variable/read"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
}
node {
  name: "cost_function/Log"
  op: "Log"
  input: "Wx_b/output_node"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
}
node {
  name: "cost_function/mul"
  op: "Mul"
  input: "shape"
  input: "cost_function/Log"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
}
node {
  name: "cost_function/Const"
  op: "Const"
  attr {
    key: "dtype"
    value {
      type: DT_INT32
    }
  }
  attr {
    key: "value"
    value {
      tensor {
        dtype: DT_INT32
        tensor_shape {
          dim {
            size: 2
          }
        }
        tensor_content: "\000\000\000\000\001\000\000\000"
      }
    }
  }
}
node {
  name: "cost_function/Sum"
  op: "Sum"
  input: "cost_function/mul"
  input: "cost_function/Const"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "Tidx"
    value {
      type: DT_INT32
    }
  }
  attr {
    key: "keep_dims"
    value {
      b: false
    }
  }
}
node {
  name: "cost_function/Neg"
  op: "Neg"
  input: "cost_function/Sum"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
}
node {
  name: "cost_function/cost_function/tags"
  op: "Const"
  attr {
    key: "dtype"
    value {
      type: DT_STRING
    }
  }
  attr {
    key: "value"
    value {
      tensor {
        dtype: DT_STRING
        tensor_shape {
        }
        string_val: "cost_function/cost_function"
      }
    }
  }
}
node {
  name: "cost_function/cost_function"
  op: "ScalarSummary"
  input: "cost_function/cost_function/tags"
  input: "cost_function/Neg"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
}
node {
  name: "train/gradients/Shape"
  op: "Const"
  attr {
    key: "dtype"
    value {
      type: DT_INT32
    }
  }
  attr {
    key: "value"
    value {
      tensor {
        dtype: DT_INT32
        tensor_shape {
          dim {
          }
        }
      }
    }
  }
}
node {
  name: "train/gradients/grad_ys_0"
  op: "Const"
  attr {
    key: "dtype"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "value"
    value {
      tensor {
        dtype: DT_FLOAT
        tensor_shape {
        }
        float_val: 1.0
      }
    }
  }
}
node {
  name: "train/gradients/Fill"
  op: "Fill"
  input: "train/gradients/Shape"
  input: "train/gradients/grad_ys_0"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "index_type"
    value {
      type: DT_INT32
    }
  }
}
node {
  name: "train/gradients/cost_function/Neg_grad/Neg"
  op: "Neg"
  input: "train/gradients/Fill"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
}
node {
  name: "train/gradients/cost_function/Sum_grad/Reshape/shape"
  op: "Const"
  attr {
    key: "dtype"
    value {
      type: DT_INT32
    }
  }
  attr {
    key: "value"
    value {
      tensor {
        dtype: DT_INT32
        tensor_shape {
          dim {
            size: 2
          }
        }
        tensor_content: "\001\000\000\000\001\000\000\000"
      }
    }
  }
}
node {
  name: "train/gradients/cost_function/Sum_grad/Reshape"
  op: "Reshape"
  input: "train/gradients/cost_function/Neg_grad/Neg"
  input: "train/gradients/cost_function/Sum_grad/Reshape/shape"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "Tshape"
    value {
      type: DT_INT32
    }
  }
}
node {
  name: "train/gradients/cost_function/Sum_grad/Shape"
  op: "Shape"
  input: "cost_function/mul"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "out_type"
    value {
      type: DT_INT32
    }
  }
}
node {
  name: "train/gradients/cost_function/Sum_grad/Tile"
  op: "Tile"
  input: "train/gradients/cost_function/Sum_grad/Reshape"
  input: "train/gradients/cost_function/Sum_grad/Shape"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "Tmultiples"
    value {
      type: DT_INT32
    }
  }
}
node {
  name: "train/gradients/cost_function/mul_grad/Shape"
  op: "Shape"
  input: "shape"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "out_type"
    value {
      type: DT_INT32
    }
  }
}
node {
  name: "train/gradients/cost_function/mul_grad/Shape_1"
  op: "Shape"
  input: "cost_function/Log"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "out_type"
    value {
      type: DT_INT32
    }
  }
}
node {
  name: "train/gradients/cost_function/mul_grad/BroadcastGradientArgs"
  op: "BroadcastGradientArgs"
  input: "train/gradients/cost_function/mul_grad/Shape"
  input: "train/gradients/cost_function/mul_grad/Shape_1"
  attr {
    key: "T"
    value {
      type: DT_INT32
    }
  }
}
node {
  name: "train/gradients/cost_function/mul_grad/Mul"
  op: "Mul"
  input: "train/gradients/cost_function/Sum_grad/Tile"
  input: "cost_function/Log"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
}
node {
  name: "train/gradients/cost_function/mul_grad/Sum"
  op: "Sum"
  input: "train/gradients/cost_function/mul_grad/Mul"
  input: "train/gradients/cost_function/mul_grad/BroadcastGradientArgs"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "Tidx"
    value {
      type: DT_INT32
    }
  }
  attr {
    key: "keep_dims"
    value {
      b: false
    }
  }
}
node {
  name: "train/gradients/cost_function/mul_grad/Reshape"
  op: "Reshape"
  input: "train/gradients/cost_function/mul_grad/Sum"
  input: "train/gradients/cost_function/mul_grad/Shape"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "Tshape"
    value {
      type: DT_INT32
    }
  }
}
node {
  name: "train/gradients/cost_function/mul_grad/Mul_1"
  op: "Mul"
  input: "shape"
  input: "train/gradients/cost_function/Sum_grad/Tile"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
}
node {
  name: "train/gradients/cost_function/mul_grad/Sum_1"
  op: "Sum"
  input: "train/gradients/cost_function/mul_grad/Mul_1"
  input: "train/gradients/cost_function/mul_grad/BroadcastGradientArgs:1"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "Tidx"
    value {
      type: DT_INT32
    }
  }
  attr {
    key: "keep_dims"
    value {
      b: false
    }
  }
}
node {
  name: "train/gradients/cost_function/mul_grad/Reshape_1"
  op: "Reshape"
  input: "train/gradients/cost_function/mul_grad/Sum_1"
  input: "train/gradients/cost_function/mul_grad/Shape_1"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "Tshape"
    value {
      type: DT_INT32
    }
  }
}
node {
  name: "train/gradients/cost_function/mul_grad/tuple/group_deps"
  op: "NoOp"
  input: "^train/gradients/cost_function/mul_grad/Reshape"
  input: "^train/gradients/cost_function/mul_grad/Reshape_1"
}
node {
  name: "train/gradients/cost_function/mul_grad/tuple/control_dependency"
  op: "Identity"
  input: "train/gradients/cost_function/mul_grad/Reshape"
  input: "^train/gradients/cost_function/mul_grad/tuple/group_deps"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "_class"
    value {
      list {
        s: "loc:@train/gradients/cost_function/mul_grad/Reshape"
      }
    }
  }
}
node {
  name: "train/gradients/cost_function/mul_grad/tuple/control_dependency_1"
  op: "Identity"
  input: "train/gradients/cost_function/mul_grad/Reshape_1"
  input: "^train/gradients/cost_function/mul_grad/tuple/group_deps"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "_class"
    value {
      list {
        s: "loc:@train/gradients/cost_function/mul_grad/Reshape_1"
      }
    }
  }
}
node {
  name: "train/gradients/cost_function/Log_grad/Reciprocal"
  op: "Reciprocal"
  input: "Wx_b/output_node"
  input: "^train/gradients/cost_function/mul_grad/tuple/control_dependency_1"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
}
node {
  name: "train/gradients/cost_function/Log_grad/mul"
  op: "Mul"
  input: "train/gradients/cost_function/mul_grad/tuple/control_dependency_1"
  input: "train/gradients/cost_function/Log_grad/Reciprocal"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
}
node {
  name: "train/gradients/Wx_b/output_node_grad/mul"
  op: "Mul"
  input: "train/gradients/cost_function/Log_grad/mul"
  input: "Wx_b/output_node"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
}
node {
  name: "train/gradients/Wx_b/output_node_grad/Sum/reduction_indices"
  op: "Const"
  attr {
    key: "dtype"
    value {
      type: DT_INT32
    }
  }
  attr {
    key: "value"
    value {
      tensor {
        dtype: DT_INT32
        tensor_shape {
          dim {
            size: 1
          }
        }
        int_val: 1
      }
    }
  }
}
node {
  name: "train/gradients/Wx_b/output_node_grad/Sum"
  op: "Sum"
  input: "train/gradients/Wx_b/output_node_grad/mul"
  input: "train/gradients/Wx_b/output_node_grad/Sum/reduction_indices"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "Tidx"
    value {
      type: DT_INT32
    }
  }
  attr {
    key: "keep_dims"
    value {
      b: false
    }
  }
}
node {
  name: "train/gradients/Wx_b/output_node_grad/Reshape/shape"
  op: "Const"
  attr {
    key: "dtype"
    value {
      type: DT_INT32
    }
  }
  attr {
    key: "value"
    value {
      tensor {
        dtype: DT_INT32
        tensor_shape {
          dim {
            size: 2
          }
        }
        tensor_content: "\377\377\377\377\001\000\000\000"
      }
    }
  }
}
node {
  name: "train/gradients/Wx_b/output_node_grad/Reshape"
  op: "Reshape"
  input: "train/gradients/Wx_b/output_node_grad/Sum"
  input: "train/gradients/Wx_b/output_node_grad/Reshape/shape"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "Tshape"
    value {
      type: DT_INT32
    }
  }
}
node {
  name: "train/gradients/Wx_b/output_node_grad/sub"
  op: "Sub"
  input: "train/gradients/cost_function/Log_grad/mul"
  input: "train/gradients/Wx_b/output_node_grad/Reshape"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
}
node {
  name: "train/gradients/Wx_b/output_node_grad/mul_1"
  op: "Mul"
  input: "train/gradients/Wx_b/output_node_grad/sub"
  input: "Wx_b/output_node"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
}
node {
  name: "train/gradients/Wx_b/add_grad/Shape"
  op: "Shape"
  input: "Wx_b/MatMul"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "out_type"
    value {
      type: DT_INT32
    }
  }
}
node {
  name: "train/gradients/Wx_b/add_grad/Shape_1"
  op: "Const"
  attr {
    key: "dtype"
    value {
      type: DT_INT32
    }
  }
  attr {
    key: "value"
    value {
      tensor {
        dtype: DT_INT32
        tensor_shape {
          dim {
            size: 1
          }
        }
        int_val: 2
      }
    }
  }
}
node {
  name: "train/gradients/Wx_b/add_grad/BroadcastGradientArgs"
  op: "BroadcastGradientArgs"
  input: "train/gradients/Wx_b/add_grad/Shape"
  input: "train/gradients/Wx_b/add_grad/Shape_1"
  attr {
    key: "T"
    value {
      type: DT_INT32
    }
  }
}
node {
  name: "train/gradients/Wx_b/add_grad/Sum"
  op: "Sum"
  input: "train/gradients/Wx_b/output_node_grad/mul_1"
  input: "train/gradients/Wx_b/add_grad/BroadcastGradientArgs"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "Tidx"
    value {
      type: DT_INT32
    }
  }
  attr {
    key: "keep_dims"
    value {
      b: false
    }
  }
}
node {
  name: "train/gradients/Wx_b/add_grad/Reshape"
  op: "Reshape"
  input: "train/gradients/Wx_b/add_grad/Sum"
  input: "train/gradients/Wx_b/add_grad/Shape"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "Tshape"
    value {
      type: DT_INT32
    }
  }
}
node {
  name: "train/gradients/Wx_b/add_grad/Sum_1"
  op: "Sum"
  input: "train/gradients/Wx_b/output_node_grad/mul_1"
  input: "train/gradients/Wx_b/add_grad/BroadcastGradientArgs:1"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "Tidx"
    value {
      type: DT_INT32
    }
  }
  attr {
    key: "keep_dims"
    value {
      b: false
    }
  }
}
node {
  name: "train/gradients/Wx_b/add_grad/Reshape_1"
  op: "Reshape"
  input: "train/gradients/Wx_b/add_grad/Sum_1"
  input: "train/gradients/Wx_b/add_grad/Shape_1"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "Tshape"
    value {
      type: DT_INT32
    }
  }
}
node {
  name: "train/gradients/Wx_b/add_grad/tuple/group_deps"
  op: "NoOp"
  input: "^train/gradients/Wx_b/add_grad/Reshape"
  input: "^train/gradients/Wx_b/add_grad/Reshape_1"
}
node {
  name: "train/gradients/Wx_b/add_grad/tuple/control_dependency"
  op: "Identity"
  input: "train/gradients/Wx_b/add_grad/Reshape"
  input: "^train/gradients/Wx_b/add_grad/tuple/group_deps"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "_class"
    value {
      list {
        s: "loc:@train/gradients/Wx_b/add_grad/Reshape"
      }
    }
  }
}
node {
  name: "train/gradients/Wx_b/add_grad/tuple/control_dependency_1"
  op: "Identity"
  input: "train/gradients/Wx_b/add_grad/Reshape_1"
  input: "^train/gradients/Wx_b/add_grad/tuple/group_deps"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "_class"
    value {
      list {
        s: "loc:@train/gradients/Wx_b/add_grad/Reshape_1"
      }
    }
  }
}
node {
  name: "train/gradients/Wx_b/MatMul_grad/MatMul"
  op: "MatMul"
  input: "train/gradients/Wx_b/add_grad/tuple/control_dependency"
  input: "weights/read"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "transpose_a"
    value {
      b: false
    }
  }
  attr {
    key: "transpose_b"
    value {
      b: true
    }
  }
}
node {
  name: "train/gradients/Wx_b/MatMul_grad/MatMul_1"
  op: "MatMul"
  input: "inp"
  input: "train/gradients/Wx_b/add_grad/tuple/control_dependency"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "transpose_a"
    value {
      b: true
    }
  }
  attr {
    key: "transpose_b"
    value {
      b: false
    }
  }
}
node {
  name: "train/gradients/Wx_b/MatMul_grad/tuple/group_deps"
  op: "NoOp"
  input: "^train/gradients/Wx_b/MatMul_grad/MatMul"
  input: "^train/gradients/Wx_b/MatMul_grad/MatMul_1"
}
node {
  name: "train/gradients/Wx_b/MatMul_grad/tuple/control_dependency"
  op: "Identity"
  input: "train/gradients/Wx_b/MatMul_grad/MatMul"
  input: "^train/gradients/Wx_b/MatMul_grad/tuple/group_deps"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "_class"
    value {
      list {
        s: "loc:@train/gradients/Wx_b/MatMul_grad/MatMul"
      }
    }
  }
}
node {
  name: "train/gradients/Wx_b/MatMul_grad/tuple/control_dependency_1"
  op: "Identity"
  input: "train/gradients/Wx_b/MatMul_grad/MatMul_1"
  input: "^train/gradients/Wx_b/MatMul_grad/tuple/group_deps"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "_class"
    value {
      list {
        s: "loc:@train/gradients/Wx_b/MatMul_grad/MatMul_1"
      }
    }
  }
}
node {
  name: "train/GradientDescent/learning_rate"
  op: "Const"
  attr {
    key: "dtype"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "value"
    value {
      tensor {
        dtype: DT_FLOAT
        tensor_shape {
        }
        float_val: 0.009999999776482582
      }
    }
  }
}
node {
  name: "train/GradientDescent/update_weights/ApplyGradientDescent"
  op: "ApplyGradientDescent"
  input: "weights"
  input: "train/GradientDescent/learning_rate"
  input: "train/gradients/Wx_b/MatMul_grad/tuple/control_dependency_1"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "_class"
    value {
      list {
        s: "loc:@weights"
      }
    }
  }
  attr {
    key: "use_locking"
    value {
      b: false
    }
  }
}
node {
  name: "train/GradientDescent/update_Variable/ApplyGradientDescent"
  op: "ApplyGradientDescent"
  input: "Variable"
  input: "train/GradientDescent/learning_rate"
  input: "train/gradients/Wx_b/add_grad/tuple/control_dependency_1"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "_class"
    value {
      list {
        s: "loc:@Variable"
      }
    }
  }
  attr {
    key: "use_locking"
    value {
      b: false
    }
  }
}
node {
  name: "train/GradientDescent"
  op: "NoOp"
  input: "^train/GradientDescent/update_weights/ApplyGradientDescent"
  input: "^train/GradientDescent/update_Variable/ApplyGradientDescent"
}
node {
  name: "init"
  op: "NoOp"
  input: "^weights/Assign"
  input: "^Variable/Assign"
}
node {
  name: "Merge/MergeSummary"
  op: "MergeSummary"
  input: "weights_1"
  input: "biases"
  input: "cost_function/cost_function"
  attr {
    key: "N"
    value {
      i: 3
    }
  }
}
node {
  name: "ArgMax/dimension"
  op: "Const"
  attr {
    key: "dtype"
    value {
      type: DT_INT32
    }
  }
  attr {
    key: "value"
    value {
      tensor {
        dtype: DT_INT32
        tensor_shape {
        }
        int_val: 1
      }
    }
  }
}
node {
  name: "ArgMax"
  op: "ArgMax"
  input: "Wx_b/output_node"
  input: "ArgMax/dimension"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "Tidx"
    value {
      type: DT_INT32
    }
  }
  attr {
    key: "output_type"
    value {
      type: DT_INT64
    }
  }
}
node {
  name: "ArgMax_1/dimension"
  op: "Const"
  attr {
    key: "dtype"
    value {
      type: DT_INT32
    }
  }
  attr {
    key: "value"
    value {
      tensor {
        dtype: DT_INT32
        tensor_shape {
        }
        int_val: 1
      }
    }
  }
}
node {
  name: "ArgMax_1"
  op: "ArgMax"
  input: "shape"
  input: "ArgMax_1/dimension"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "Tidx"
    value {
      type: DT_INT32
    }
  }
  attr {
    key: "output_type"
    value {
      type: DT_INT64
    }
  }
}
node {
  name: "Equal"
  op: "Equal"
  input: "ArgMax"
  input: "ArgMax_1"
  attr {
    key: "T"
    value {
      type: DT_INT64
    }
  }
}
node {
  name: "Cast"
  op: "Cast"
  input: "Equal"
  attr {
    key: "DstT"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "SrcT"
    value {
      type: DT_BOOL
    }
  }
}
node {
  name: "Const"
  op: "Const"
  attr {
    key: "dtype"
    value {
      type: DT_INT32
    }
  }
  attr {
    key: "value"
    value {
      tensor {
        dtype: DT_INT32
        tensor_shape {
          dim {
            size: 1
          }
        }
        int_val: 0
      }
    }
  }
}
node {
  name: "Mean"
  op: "Mean"
  input: "Cast"
  input: "Const"
  attr {
    key: "T"
    value {
      type: DT_FLOAT
    }
  }
  attr {
    key: "Tidx"
    value {
      type: DT_INT32
    }
  }
  attr {
    key: "keep_dims"
    value {
      b: false
    }
  }
}
versions {
  producer: 26
}
