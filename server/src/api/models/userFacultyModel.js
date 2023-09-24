import mongoose from "mongoose";
import sanitizerPlugin from 'mongoose-sanitizer'

//estimated student schema
const userFacultyModel = mongoose.Schema({
     id: { type: String },
     name: { type: String, required: true },
     email: { type: String, required: true },
     password: { type: String, required: true },
     branch: { type: String, required: true },
     subjects: { type: String, required: true },
     designation: { type: String, required: true },
     education: { type: String, required: true },
     role: { type: String, default: 'faculty'},
     bio: { type: String },
     intrest: { type: String },
     mobile: { type: String}
})


// sanitize schema
userFacultyModel.plugin(sanitizerPlugin);


export default mongoose.model("userFaculties",userFacultyModel);