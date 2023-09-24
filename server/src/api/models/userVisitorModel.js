import mongoose from "mongoose";
import sanitizerPlugin from 'mongoose-sanitizer'

//estimated visitor schema
const userVisitorModel = mongoose.Schema({
     id: { type: String },
     name: { type: String, required: true },
     email: { type: String, required: true },
     password: { type: String, required: true },
     role: { type: String, default: 'visitor'},
     bio: { type: String },
})


// sanitize schema
userVisitorModel.plugin(sanitizerPlugin);

export default mongoose.model("userVisitors", userVisitorModel);