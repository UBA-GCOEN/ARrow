import mongoose from "mongoose";
import sanitizerPlugin from 'mongoose-sanitizer'


//estimated admin schema
const userAdminModel = new mongoose.Schema({
  id: { type: String },
  name: { type: String, required: true },
  email: { type: String, required: true },
  password: { type: String, required: true },
  role: { type: String, default: 'admin'},
});

// sanitize schema
userAdminModel.plugin(sanitizerPlugin);



export default mongoose.model("userAdmins", userAdminModel);
